using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAppP2P.Api;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Messages;
using WebAppP2P.Dto;
using WebAppP2P.Services;

namespace WebAppP2P.WebSockets.InternalMessages
{
    public class InternalMessageMediator : IInternalMessageMediator
    {
        private static readonly ConcurrentDictionary<Guid, string> _mapConnectionsToPublicKey = new ConcurrentDictionary<Guid, string>();
        private static readonly ConcurrentDictionary<string, string> _mapPublicKeyToPrivateKey = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<Guid,WebSocket> _connections = new ConcurrentDictionary<Guid, WebSocket>();

        private readonly ILogger _logger;
        private readonly IPeerCommunicationService _peerCommunicationService;
        private readonly IMessageStore _messageStore;
        private readonly IMessageDecryptor _messageDecryptor;
        private readonly IEncryptedMessageBuilder _messageBuilder;
        private readonly IInternalMessageSender _internalMessageSender;
        private readonly IMapper _mapper;

        public InternalMessageMediator(ILogger<InternalMessageMediator> logger,
                                      IPeerCommunicationService peerCommunicationService,
                                      IMessageStore messageStore,
                                      IMessageDecryptor messageDecrypter,
                                      IEncryptedMessageBuilder messageBuilder,
                                      IInternalMessageSender internalMessageSender,
                                      IMapper mapper
            )
        {
            _logger = logger;
            _peerCommunicationService = peerCommunicationService;
            _messageStore = messageStore;
            _messageDecryptor = messageDecrypter;
            _messageBuilder = messageBuilder;
            _internalMessageSender = internalMessageSender;
            _mapper = mapper;
        }

        public void Start(IWebSocketConnection webSocketConnection)
        {
            if(!_connections.TryAdd(webSocketConnection.ConnectionId, webSocketConnection.WebSocket))
            {
                throw new Exception("Not expected behaviour");
            }

            _logger.LogInformation("New connection {0}", webSocketConnection.ConnectionId);
        }

        public async Task ReceiveAsync(IWebSocketConnection webSocketConnection, WebSocketMessageContract message)
        {
            var connectionId = webSocketConnection.ConnectionId;

            if(message == null)
            {
                throw new ArgumentNullException("Message cannot be null");
            }

            _logger.LogInformation("Receive message {0}", message.Type);

            switch (message.Type)
            {
                case WebSocketMessageRequestTypes.CONFIG:
                    ReceiveConfigMessage((ConfigMessage)message.Data, connectionId);
                    break;
                case WebSocketMessageRequestTypes.MESSAGE:
                    var clientInternalMessage = (ClientInternalMessage)message.Data;
                    try
                    {
                        await ReceiveInternalMessageAsync(webSocketConnection, clientInternalMessage);
                    }
                    catch (Exception ex)
                    {
                        await _internalMessageSender.SendAsync(
                            new WebSocketConnection(webSocketConnection.WebSocket,connectionId), 
                            new WebSocketMessageContract()
                        {
                            Type = WebSocketMessageResponseTypes.INTERNAL_MESSAGE_ERROR,
                            Data = new InternalMessageError()
                            {
                                CorrelationId = clientInternalMessage.CorrelationId.Value,
                                Description = "Error occured: " + ex.Message
                            }
                        });      
                    }
                    break;
                case WebSocketMessageRequestTypes.SYNCHRONIZATION:
                    var clientSynchronizationMessage = (ClientSynchronizationMessage)message.Data;
                    await ReceiveSynchronizationMessage(webSocketConnection,clientSynchronizationMessage);
                    break;
                default:
                    throw new ArgumentException(nameof(message.Type));
            }
       
        }

        public async Task SendAsync(EncryptedMessage encryptedMessage)
        {
            if (!_mapConnectionsToPublicKey.Any(p => p.Value == encryptedMessage.To))
            {
                return;
            }
            var decryptedMessage = _messageDecryptor.Decrypt(encryptedMessage, GetPrivateKeyFromPublicKey(encryptedMessage.To),true);

            var internalMessage = _mapper.Map<InternalMessage>(decryptedMessage);
            internalMessage.DateTime = DateTimeOffset.FromUnixTimeSeconds(internalMessage.Timestamp).ToString();
            internalMessage.Title = internalMessage.Title;
            internalMessage.Content = internalMessage.Content;
            internalMessage.Id = internalMessage.Id.GetHashCode().ToString();

            var connections = GetConnections(internalMessage.To);
            var tasks = connections.Select(async (connId) =>
            {
                WebSocket webSocket;
                if(!_connections.TryGetValue(connId,out webSocket))
                {
                    throw new Exception("Not expected behaviour");
                }
                await _internalMessageSender.SendAsync(new WebSocketConnection(webSocket,connId), new WebSocketMessageContract()
                {
                    Type = WebSocketMessageResponseTypes.INTERNAL_MESSAGE,
                    Data = internalMessage
                });
            });
            await Task.WhenAll(tasks);
        }

        public void Close(IWebSocketConnection webSocketConnection)
        {
            var connectionId = webSocketConnection.ConnectionId;
            string publicKey;
            if(!_mapConnectionsToPublicKey.TryRemove(connectionId, out publicKey))
            {
                _logger.LogError("TryRemove failed for connection id: {0}", connectionId);
            }
            else if(GetConnections(publicKey).Count() <= 1)
            {
                _mapPublicKeyToPrivateKey.TryRemove(publicKey, out string privateKey);
            }
            _connections.TryRemove(connectionId, out WebSocket value2);  
        }

        private async Task ReceiveSynchronizationMessage(IWebSocketConnection webSocketConnection,ClientSynchronizationMessage message)
        {
            string publicKey;
            if (!_mapConnectionsToPublicKey.TryGetValue(webSocketConnection.ConnectionId, out publicKey))
            {
                return;
            }
           
            var messages = _messageStore.Get(
                e =>
                    e.Timestamp <= message.TimestampTo &&
                    e.Timestamp > message.TimestampFrom &&
                    (e.To == publicKey || e.From == publicKey)
                    );
            //var tasks = messages.Select(e => SendAsync(e));//TODO Synchro also SENT msges not only INBOX
            var tasks = messages.Select(async (encryptedMessage) =>
            {
                var publicKeyIsReciever = encryptedMessage.To == publicKey;
                var decryptedMessage = _messageDecryptor.Decrypt(
                    encryptedMessage, 
                    GetPrivateKeyFromPublicKey(publicKeyIsReciever ? encryptedMessage.To : encryptedMessage.From)
                    , publicKeyIsReciever);

                var internalMessage = _mapper.Map<InternalMessage>(decryptedMessage);
                internalMessage.DateTime = DateTimeOffset.FromUnixTimeSeconds(internalMessage.Timestamp).ToString();
                internalMessage.Title = internalMessage.Title;
                internalMessage.Content = internalMessage.Content;
                internalMessage.Id = internalMessage.Id.GetHashCode().ToString();

                await _internalMessageSender.SendAsync(webSocketConnection, new WebSocketMessageContract()
                {
                    Type = WebSocketMessageResponseTypes.INTERNAL_MESSAGE,
                    Data = internalMessage
                });
            });
            await Task.WhenAll(tasks);
            await _internalMessageSender.SendAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageResponseTypes.END_OF_SYNCHRONIZATION,
                Data = message
            });
        }

        private async Task ReceiveInternalMessageAsync(IWebSocketConnection webSocketConnection, ClientInternalMessage clientInternalMessage)
        {
            _logger.LogInformation("Received internal message to {0}", clientInternalMessage.To);
            GetConnections(clientInternalMessage.To).ToList().ForEach(id =>
            {
                _logger.LogInformation("-Active connection for reciever: {0}", id);
            });
            _logger.LogInformation("-No more active connections for reciever");
            

            var connectionId = webSocketConnection.ConnectionId;
            var from = _mapConnectionsToPublicKey.Where(m => m.Key == connectionId).Select(m => m.Value).SingleOrDefault();

            var message = _messageBuilder.AddContent(clientInternalMessage.Content)
                                .AddReciever(clientInternalMessage.To)
                                .AddSender(from)
                                .AddTitle(clientInternalMessage.Title)
                                .AddContent(clientInternalMessage.Content)
                                .EncryptAndBuild(GetPrivateKeyFromConnectionId(connectionId));

            var added = _messageStore.TryAddAsync(message);
            if (!added.Result)
            {
                throw new InvalidOperationException();
            }

            var internalMessage = _mapper.Map<InternalMessage>(clientInternalMessage);
            internalMessage.From = from;
            internalMessage.DateTime = DateTimeOffset.FromUnixTimeSeconds(message.Timestamp).ToString();
            internalMessage.Timestamp = message.Timestamp;
            internalMessage.Id = message.Id.GetHashCode().ToString();

            await SendAsync(internalMessage, connectionId);
            await SendConfirmationToSender(webSocketConnection, clientInternalMessage, internalMessage);
            await _peerCommunicationService.SendAsync(Endpoints.Messages, message);
        }

        private async Task SendAsync(InternalMessage internalMessage, Guid connectionId)
        {
            var connections = GetConnections(internalMessage.To);
            var tasks = connections.Where(c => c != connectionId).Select(async (connId) =>
            {
                WebSocket webSocketConnection;
                _connections.TryGetValue(connId, out webSocketConnection);
                await _internalMessageSender.SendAsync(
                    new WebSocketConnection(webSocketConnection,connId), 
                    new WebSocketMessageContract()
                {
                    Type = WebSocketMessageResponseTypes.INTERNAL_MESSAGE,
                    Data = internalMessage
                });
            });

            await Task.WhenAll(tasks);
        }

        private async Task SendConfirmationToSender(IWebSocketConnection webSocketConnection, ClientInternalMessage clientInternalMessage, InternalMessage internalMessage)
        {
            if (!clientInternalMessage.CorrelationId.HasValue)
            {
                return;
            }

            var confirmationMessage = _mapper.Map<InternalMessageConfirmation>(clientInternalMessage);
            confirmationMessage.Id = internalMessage.Id;
            confirmationMessage.Timestamp = internalMessage.Timestamp;
            confirmationMessage.DateTime = internalMessage.DateTime;
            await _internalMessageSender.SendAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageResponseTypes.INTERNAL_MESSAGE_CONFIRMATION,
                Data = confirmationMessage
            });
        }

        private void ReceiveConfigMessage(ConfigMessage configMessage, Guid connectionId)
        {
            _mapConnectionsToPublicKey.TryAdd(connectionId, configMessage.PublicKey);
            _mapPublicKeyToPrivateKey.TryAdd(configMessage.PublicKey, configMessage.PrivateKey);
        }

        private IEnumerable<Guid> GetConnections(string publicKey)
        {
            return _mapConnectionsToPublicKey.Where(x => x.Value == publicKey).Select(x => x.Key);
        }

        private string GetPrivateKeyFromConnectionId(Guid connectionId)
        {
            var publicKey = _mapConnectionsToPublicKey.Single(c => c.Key == connectionId).Value;
            return GetPrivateKeyFromPublicKey(publicKey);
        }

        private string GetPrivateKeyFromPublicKey(string publicKey)
        {
            return _mapPublicKeyToPrivateKey.Single(p => p.Key == publicKey).Value;
        }

        

    }
}
