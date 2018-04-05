using System;
using System.Net.WebSockets;

namespace WebAppP2P.WebSockets
{
    public interface IWebSocketConnection
    {
        Guid ConnectionId { get; }
        WebSocket WebSocket { get; }
    }
}