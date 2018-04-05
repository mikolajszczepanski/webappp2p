using System.Net.WebSockets;
using System.Threading.Tasks;
using WebAppP2P.Dto;

namespace WebAppP2P.WebSockets.InternalMessages
{
    public interface IInternalMessageSender
    {
        Task SendAsync(IWebSocketConnection webSocketConnection, WebSocketMessageContract webSocketMessage);
    }
}