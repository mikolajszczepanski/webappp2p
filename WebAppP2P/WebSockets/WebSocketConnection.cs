using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebAppP2P.WebSockets
{
    public class WebSocketConnection : IWebSocketConnection
    {
        public WebSocket WebSocket { get; }
        public Guid ConnectionId { get; }

        public WebSocketConnection(WebSocket webSocket, Guid connectionId)
        {
            WebSocket = webSocket;
            ConnectionId = connectionId;
        }
    }
}
