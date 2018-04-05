using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebAppP2P.WebSockets
{
    public interface IWebSocketConnectionHandler
    {
        Task OnCloseAsync(IWebSocketConnection webSocketConnection);
        Task OnReceiveAsync(IWebSocketConnection webSocketConnection);
        void OnStart(IWebSocketConnection webSocketConnection);
    }
}