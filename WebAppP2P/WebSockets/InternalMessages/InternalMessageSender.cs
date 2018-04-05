using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAppP2P.Dto;

namespace WebAppP2P.WebSockets.InternalMessages
{
    public class InternalMessageSender : IInternalMessageSender
    {
        private readonly ILogger _logger;

        public InternalMessageSender(ILogger<InternalMessageSender> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(IWebSocketConnection webSocketConnection, WebSocketMessageContract webSocketMessage)
        {
            _logger.LogInformation("Sending message of type {0} on connection {1}", webSocketMessage.Type, webSocketConnection.ConnectionId);
            var text = JsonConvert.SerializeObject(webSocketMessage);
            if (webSocketConnection.WebSocket.State != WebSocketState.Open)
            {
                return;
            }
            var textToByteArray = Encoding.UTF8.GetBytes(text);
            var buffer = new ArraySegment<byte>(
                array: textToByteArray,
                offset: 0,
                count: textToByteArray.Length
                );
            await webSocketConnection.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
