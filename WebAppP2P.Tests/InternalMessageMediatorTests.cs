using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Api;
using WebAppP2P.Core.Messages;
using WebAppP2P.Dto;
using WebAppP2P.Services;
using WebAppP2P.Tests.Helpers;
using WebAppP2P.WebSockets;
using WebAppP2P.WebSockets.InternalMessages;
using Xunit;

namespace WebAppP2P.Tests
{
    public class InternalMessageMediatorTests
    {
        private MapperConfiguration _mapperConfiguration;

        public InternalMessageMediatorTests()
        {
            _mapperConfiguration = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
        }

        [Fact]
        public async Task InternalMessageMediator_SendAsync_Should_Forward_Message_When_To_User_Is_Connected()
        {
            var mockedData = GetMockedMediatorWithDependencies();
            var internalMessageMediator = mockedData.InternalMessageMediator;
            var websocketMock = new Mock<WebSocket>();
            var webSocketConnection = new WebSocketConnection(
                websocketMock.Object,
                Guid.NewGuid()
                );

            internalMessageMediator.Start(webSocketConnection);
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.CONFIG,
                Data = new Dto.ConfigMessage()
                {
                    PrivateKey = "to_test priv key",
                    PublicKey = "to_test"
                }
            });

            await internalMessageMediator.SendAsync(new EncryptedMessage()
            {
                To = "to_test",
                From = "from_test",
                Timestamp = 1,
                Id = "234",
                Content = "test content",
                Title = "test title"
            });

            mockedData.InternalMessageSender.Verify(s =>
                s.SendAsync(
                    It.Is<IWebSocketConnection>(w => w.WebSocket == webSocketConnection.WebSocket),
                    It.Is<WebSocketMessageContract>(m =>
                        m.Type == WebSocketMessageResponseTypes.INTERNAL_MESSAGE &&
                        m.Data is InternalMessage &&
                        (m.Data as InternalMessage).Timestamp == 1 &&
                        (m.Data as InternalMessage).To == "to_test" &&
                        (m.Data as InternalMessage).From == "from_test" &&
                        (m.Data as InternalMessage).Title == "DECRYPTED_TITLE_test title" &&
                        (m.Data as InternalMessage).Content == "DECRYPTED_CONTENT_test content"
                    )
                    ), Times.Once);
        }

        [Fact]
        public async Task InternalMessageMediator_SendAsync_Should_Not_Forward_Message_When_To_User_Is_Not_Connected()
        {
            var mockedData = GetMockedMediatorWithDependencies();
            var internalMessageMediator = mockedData.InternalMessageMediator;
            var websocketMock = new Mock<WebSocket>();
            var webSocketConnection = new WebSocketConnection(
                websocketMock.Object,
                Guid.NewGuid()
                );

            internalMessageMediator.Start(webSocketConnection);

            await internalMessageMediator.SendAsync(new EncryptedMessage()
            {
                To = "to_test_X",
                From = "from_test_X",
                Timestamp = 1,
                Id = "234",
                Content = "test content",
                Title = "test title"
            });

            mockedData.InternalMessageSender.Verify(s =>
                s.SendAsync(
                    It.IsAny<IWebSocketConnection>(),
                    It.IsAny<WebSocketMessageContract>()
                    ), Times.Never);
        }

        [Fact]
        public async Task InternalMessageMediator_ReceiveAsync_Should_Send_Message_To_Reciever_User_When_Is_Connected()
        {
            var peerCommunicationService = new Mock<IPeerCommunicationService>();
            var messageStore = new MockMessageStore();
            var messageDecryptor = new Mock<IMessageDecryptor>();
            var internalMessageSender = new Mock<IInternalMessageSender>();
            var messageBuilder = new MockEncryptedMessageBuilder();

            var internalMessageMediator = new InternalMessageMediator(
                new Mock<ILogger<InternalMessageMediator>>().Object,
                peerCommunicationService.Object,
                messageStore,
                messageDecryptor.Object,
                messageBuilder,
                internalMessageSender.Object,
                _mapperConfiguration.CreateMapper()
                );
            var websocketMock = new Mock<WebSocket>();
            var webSocketConnection = new WebSocketConnection(
                websocketMock.Object,
                Guid.NewGuid()
                );
            var websocketMockReciever = new Mock<WebSocket>();
            var webSocketConnectionReciever = new WebSocketConnection(
                websocketMockReciever.Object,
                Guid.NewGuid()
                );

            internalMessageMediator.Start(webSocketConnection);
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.CONFIG,
                Data = new Dto.ConfigMessage()
                {
                    PrivateKey = "privKey",
                    PublicKey = "pubKey"
                }
            });

            internalMessageMediator.Start(webSocketConnectionReciever);
            await internalMessageMediator.ReceiveAsync(webSocketConnectionReciever, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.CONFIG,
                Data = new Dto.ConfigMessage()
                {
                    PrivateKey = "privKeyReciever",
                    PublicKey = "pubKeyReciever"
                }
            });


            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.MESSAGE,
                Data = new Dto.ClientInternalMessage()
                {
                    Content = "test content",
                    To = "pubKeyReciever",
                    CorrelationId = 123,
                    Title = "test title"
                }
            });

            internalMessageSender.Verify(s =>
                s.SendAsync(
                    It.Is<IWebSocketConnection>(w => w.WebSocket == webSocketConnectionReciever.WebSocket),
                    It.Is<WebSocketMessageContract>(m =>
                        m.Type == WebSocketMessageResponseTypes.INTERNAL_MESSAGE &&
                        m.Data is InternalMessage &&
                        (m.Data as InternalMessage).Title == "test title" &&
                        (m.Data as InternalMessage).Content == "test content" &&
                        (m.Data as InternalMessage).To == "pubKeyReciever" &&
                        (m.Data as InternalMessage).From == "pubKey"
                    )
                    ), Times.Once);
        }

        [Fact]
        public async Task InternalMessageMediator_ReceiveAsync_Should_Try_Add_Message_To_Store()
        {
            var peerCommunicationService = new Mock<IPeerCommunicationService>();
            var messageStore = new Mock<IMessageStore>();
            var messageDecryptor = new Mock<IMessageDecryptor>();
            var internalMessageSender = new Mock<IInternalMessageSender>();
            var messageBuilder = new MockEncryptedMessageBuilder();

            messageStore.Setup(s => s.TryAddAsync(It.IsAny<EncryptedMessage>())).Returns(Task.FromResult(true));
            var internalMessageMediator = new InternalMessageMediator(
                new Mock<ILogger<InternalMessageMediator>>().Object,
                peerCommunicationService.Object,
                messageStore.Object,
                messageDecryptor.Object,
                messageBuilder,
                internalMessageSender.Object,
                _mapperConfiguration.CreateMapper()
                );
            var websocketMock = new Mock<WebSocket>();
            var webSocketConnection = new WebSocketConnection(
                websocketMock.Object,
                Guid.NewGuid()
                );

            internalMessageMediator.Start(webSocketConnection);
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.CONFIG,
                Data = new Dto.ConfigMessage()
                {
                    PrivateKey = "privKey",
                    PublicKey = "pubKey"
                }
            });
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.MESSAGE,
                Data = new Dto.ClientInternalMessage()
                {
                    Content = "test content",
                    To = "xxx",
                    CorrelationId = 123,
                    Title = "test title"
                }
            });

            messageStore.Verify(s => s.TryAddAsync(It.Is<EncryptedMessage>(
                m => m.To == "xxx" &&
                m.From == "pubKey" &&
                m.Content == "ENCRYPTED_CONTENT_test content" &&
                m.Title == "ENCRYPTED_TITLE_test title"
                )),Times.Once);
        }

        [Fact]
        public async Task InternalMessageMediator_ReceiveAsync_Should_Send_Stored_Messages_When_Client_Request_Synchronization()
        {
            var peerCommunicationService = new Mock<IPeerCommunicationService>();
            var messageStore = new MockMessageStore();
            var messageDecryptor = new Mock<IMessageDecryptor>();
            var internalMessageSender = new Mock<IInternalMessageSender>();
            var messageBuilder = new MockEncryptedMessageBuilder();

            messageDecryptor.Setup(d => d.Decrypt(
               It.IsAny<EncryptedMessage>(),
               It.IsAny<string>(),
               It.IsAny<bool>()
               ))
               .Returns<EncryptedMessage, string, bool>((msg, priv, privReciever) => new DecryptedMessageDto()
               {
                   Id = msg.Id,
                   From = msg.From,
                   To = msg.To,
                   Content = "DECRYPTED_CONTENT_" + msg.Content,
                   Title = "DECRYPTED_TITLE_" + msg.Title,
                   Timestamp = msg.Timestamp
               }
               );
            messageStore.State = new Dictionary<string, EncryptedMessage>()
            {
                { "1", new EncryptedMessage(){
                        Content = "msg",
                        Title = "tit",
                        Timestamp = 20,
                        From = "pubKey",
                        To = "xyz",
                        FromKey = "",
                        Id = "1",
                        IV = "",
                        Nonce = 0,
                        ToKey = ""
                    }
                },
                { "2", new EncryptedMessage(){
                        Content = "msg",
                        Title = "tit",
                        Timestamp = 21,
                        From = "pubKey",
                        To = "xyz",
                        FromKey = "",
                        Id = "2",
                        IV = "",
                        Nonce = 0,
                        ToKey = ""
                    }
                },
                { "3", new EncryptedMessage(){
                        Content = "msg",
                        Title = "tit",
                        Timestamp = 3,
                        From = "pubKey",
                        To = "xyz",
                        FromKey = "",
                        Id = "3",
                        IV = "",
                        Nonce = 0,
                        ToKey = ""
                    }
                },
                { "4", new EncryptedMessage(){
                        Content = "msg2",
                        Title = "tit2",
                        Timestamp = 13,
                        From = "xyz",
                        To = "pubKey",
                        FromKey = "",
                        Id = "4",
                        IV = "",
                        Nonce = 0,
                        ToKey = ""
                    }
                }
            };
            var internalMessageMediator = new InternalMessageMediator(
                new Mock<ILogger<InternalMessageMediator>>().Object,
                peerCommunicationService.Object,
                messageStore,
                messageDecryptor.Object,
                messageBuilder,
                internalMessageSender.Object,
                _mapperConfiguration.CreateMapper()
                );
            var websocketMock = new Mock<WebSocket>();
            var webSocketConnection = new WebSocketConnection(
                websocketMock.Object,
                Guid.NewGuid()
                );

            internalMessageMediator.Start(webSocketConnection);
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.CONFIG,
                Data = new Dto.ConfigMessage()
                {
                    PrivateKey = "privKey",
                    PublicKey = "pubKey"
                }
            });
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.SYNCHRONIZATION,
                Data = new Dto.ClientSynchronizationMessage()
                {
                    TimestampFrom = 10,
                    TimestampTo = 20
                }
            });

            internalMessageSender.Verify(s => s.SendAsync(
                It.Is<IWebSocketConnection>(c => c.WebSocket == webSocketConnection.WebSocket),
                It.Is<WebSocketMessageContract>(
                    m => m.Type == WebSocketMessageResponseTypes.INTERNAL_MESSAGE &&
                        m.Data is InternalMessage &&
                        (m.Data as InternalMessage).To == "xyz" &&
                        (m.Data as InternalMessage).From == "pubKey" &&
                        (m.Data as InternalMessage).Content == "DECRYPTED_CONTENT_msg" &&
                        (m.Data as InternalMessage).Title == "DECRYPTED_TITLE_tit" &&
                        (m.Data as InternalMessage).Timestamp == 21 || (m.Data as InternalMessage).Timestamp == 3
                    )), Times.Never);

            internalMessageSender.Verify(s => s.SendAsync(
                It.Is<IWebSocketConnection>(c => c.WebSocket == webSocketConnection.WebSocket), 
                It.Is<WebSocketMessageContract>(
                    m => m.Type == WebSocketMessageResponseTypes.INTERNAL_MESSAGE &&
                        m.Data is InternalMessage &&
                        (m.Data as InternalMessage).To == "xyz" &&
                        (m.Data as InternalMessage).From == "pubKey" &&
                        (m.Data as InternalMessage).Content == "DECRYPTED_CONTENT_msg" &&
                        (m.Data as InternalMessage).Title == "DECRYPTED_TITLE_tit" &&
                        (m.Data as InternalMessage).Timestamp == 20
                    )),Times.Once);
            internalMessageSender.Verify(s => s.SendAsync(
                It.Is<IWebSocketConnection>(c => c.WebSocket == webSocketConnection.WebSocket),
                It.Is<WebSocketMessageContract>(
                    m => m.Type == WebSocketMessageResponseTypes.INTERNAL_MESSAGE &&
                        m.Data is InternalMessage &&
                        (m.Data as InternalMessage).To == "pubKey" &&
                        (m.Data as InternalMessage).From == "xyz" &&
                        (m.Data as InternalMessage).Content == "DECRYPTED_CONTENT_msg2" &&
                        (m.Data as InternalMessage).Title == "DECRYPTED_TITLE_tit2" &&
                        (m.Data as InternalMessage).Timestamp == 13
                    )), Times.Once);
        }

        [Fact]
        public async Task InternalMessageMediator_ReceiveAsync_Should_Push_Message_To_Peer_Service()
        {
            var peerCommunicationService = new Mock<IPeerCommunicationService>();
            var messageStore = new MockMessageStore();
            var messageDecryptor = new Mock<IMessageDecryptor>();
            var internalMessageSender = new Mock<IInternalMessageSender>();
            var messageBuilder = new MockEncryptedMessageBuilder();

            peerCommunicationService.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<object>()));
            var internalMessageMediator = new InternalMessageMediator(
                new Mock<ILogger<InternalMessageMediator>>().Object,
                peerCommunicationService.Object,
                messageStore,
                messageDecryptor.Object,
                messageBuilder,
                internalMessageSender.Object,
                _mapperConfiguration.CreateMapper()
                );
            var websocketMock = new Mock<WebSocket>();
            var webSocketConnection = new WebSocketConnection(
                websocketMock.Object,
                Guid.NewGuid()
                );

            internalMessageMediator.Start(webSocketConnection);
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.CONFIG,
                Data = new Dto.ConfigMessage()
                {
                    PrivateKey = "privKey",
                    PublicKey = "pubKey"
                }
            });
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.MESSAGE,
                Data = new Dto.ClientInternalMessage()
                {
                    Content = "test content",
                    To = "xxx",
                    CorrelationId = 123,
                    Title = "test title"
                }
            });

            peerCommunicationService.Verify(s => s.SendAsync(
                It.Is<string>(m => m == Endpoints.Messages), 
                It.Is<EncryptedMessage>(
                    m => m.To == "xxx" &&
                    m.From == "pubKey" &&
                    m.Content == "ENCRYPTED_CONTENT_test content" &&
                    m.Title == "ENCRYPTED_TITLE_test title"
                )), Times.Once);
        }

        [Fact]
        public async Task InternalMessageMediator_ReceiveAsync_Should_Send_Confirmation_When_Got_Internal_Message()
        {
            var peerCommunicationService = new Mock<IPeerCommunicationService>();
            var messageStore = new MockMessageStore();
            var messageDecryptor = new Mock<IMessageDecryptor>();
            var internalMessageSender = new Mock<IInternalMessageSender>();
            var messageBuilder = new MockEncryptedMessageBuilder();

            var internalMessageMediator = new InternalMessageMediator(
                new Mock<ILogger<InternalMessageMediator>>().Object,
                peerCommunicationService.Object,
                messageStore,
                messageDecryptor.Object,
                messageBuilder,
                internalMessageSender.Object,
                _mapperConfiguration.CreateMapper()
                );
            var websocketMock = new Mock<WebSocket>();
            var webSocketConnection = new WebSocketConnection(
                websocketMock.Object,
                Guid.NewGuid()
                );

            internalMessageMediator.Start(webSocketConnection);
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.CONFIG,
                Data = new Dto.ConfigMessage()
                {
                    PrivateKey = "privKey",
                    PublicKey = "pubKey"
                }
            });
            await internalMessageMediator.ReceiveAsync(webSocketConnection, new WebSocketMessageContract()
            {
                Type = WebSocketMessageRequestTypes.MESSAGE,
                Data = new Dto.ClientInternalMessage()
                {
                    Content = "test content",
                    To = "xxx",
                    CorrelationId = 123,
                    Title = "test title"
                }
            });

            internalMessageSender.Verify(s =>
                s.SendAsync(
                    It.Is<IWebSocketConnection>(w => w.WebSocket == webSocketConnection.WebSocket),
                    It.Is<WebSocketMessageContract>(m => 
                        m.Type == WebSocketMessageResponseTypes.INTERNAL_MESSAGE_CONFIRMATION &&
                        m.Data is InternalMessageConfirmation &&
                        (m.Data as InternalMessageConfirmation).CorrelationId == 123
                    )
                    ), Times.Once);
        }

        private (IInternalMessageMediator InternalMessageMediator, Mock<IInternalMessageSender> InternalMessageSender) GetMockedMediatorWithDependencies()
        {
            var peerCommunicationService = new Mock<IPeerCommunicationService>();
            var messageStore = new MockMessageStore();
            var messageDecryptor = new Mock<IMessageDecryptor>();
            messageDecryptor.Setup(d => d.Decrypt(
                It.IsAny<EncryptedMessage>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                ))
                .Returns<EncryptedMessage, string, bool>((msg, priv, privReciever) => new DecryptedMessageDto()
                {
                    Id = msg.Id,
                    From = msg.From,
                    To = msg.To,
                    Content = "DECRYPTED_CONTENT_" + msg.Content,
                    Title = "DECRYPTED_TITLE_" + msg.Title,
                    Timestamp = msg.Timestamp
                }
                );
            var internalMessageSender = new Mock<IInternalMessageSender>();
            var messageBuilder = new MockEncryptedMessageBuilder();

            var internalMessageMediator = new InternalMessageMediator(
                new Mock<ILogger<InternalMessageMediator>>().Object,
                peerCommunicationService.Object,
                messageStore,
                messageDecryptor.Object,
                messageBuilder,
                internalMessageSender.Object,
                _mapperConfiguration.CreateMapper()
                );

            return (internalMessageMediator, internalMessageSender);
        }
    }
}
