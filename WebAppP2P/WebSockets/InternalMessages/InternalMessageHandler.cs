using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAppP2P.Dto;
using WebAppP2P.WebSockets;

namespace WebAppP2P.WebSockets.InternalMessages
{
    public class InternalMessageWebsocketConnectionHandler : IWebSocketConnectionHandler
    {
        private const int BufferSize = 1024 * 8;
        private readonly IInternalMessageMediator _mediator;
        private readonly IInternalMessageSender _internalMessageSender;
        private readonly ILogger _logger;

        public InternalMessageWebsocketConnectionHandler(
            IInternalMessageMediator mediator, 
            IInternalMessageSender internalMessageSender,
            ILogger<InternalMessageWebsocketConnectionHandler> logger
            )
        {
            _mediator = mediator;
            _internalMessageSender = internalMessageSender;
            _logger = logger;
        }

        public async Task OnCloseAsync(IWebSocketConnection webSocketConnection)
        {
            _logger.LogInformation("Close websocket connection {0}", webSocketConnection.ConnectionId.ToString());
            _mediator.Close(webSocketConnection);
            await webSocketConnection.WebSocket.CloseAsync(
                webSocketConnection.WebSocket.CloseStatus.Value,
                webSocketConnection.WebSocket.CloseStatusDescription, 
                CancellationToken.None);
        }

        public async Task OnReceiveAsync(IWebSocketConnection webSocketConnection)
        {
            if (webSocketConnection.WebSocket.State != WebSocketState.Open)
            {
                return;
            }
            _logger.LogInformation("Receive message on websocket connection {0}", webSocketConnection.ConnectionId.ToString());
            var buffer = WebSocket.CreateServerBuffer(BufferSize);
            WebSocketReceiveResult result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                do
                {
                    result = await webSocketConnection.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.Count > 0)
                    {
                        memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                } while (!result.EndOfMessage && result.CloseStatus == null);

                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    var message = streamReader.ReadToEnd();
                    var conv = new InternalMessageJsonDeserializer();
                    try
                    {
                        await _mediator.ReceiveAsync(webSocketConnection, conv.Deserialize(message));
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.LogError(ex, ex.Message);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        _logger.LogError(ex.StackTrace);
                        _logger.LogError("Message = {0}", message);
                        await _internalMessageSender.SendAsync(webSocketConnection, new WebSocketMessageContract()
                        {
                            Type = WebSocketMessageResponseTypes.ERROR,
                            Data = new Error()
                            {
                                Description = "Fatal error occured - " + ex.Message,
                                Exception = ex
                            }
                        });
                    }
                }
            }
        }

        public void OnStart(IWebSocketConnection webSocketConnection)
        {
            _logger.LogInformation("Start websocket connection {0}", webSocketConnection.ConnectionId.ToString());
            _mediator.Start(webSocketConnection);
        }
    }
}
