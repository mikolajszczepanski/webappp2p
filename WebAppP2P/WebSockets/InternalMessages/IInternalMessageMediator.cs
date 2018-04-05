using System.Threading.Tasks;
using WebAppP2P.Core.Messages;
using WebAppP2P.Dto;
using WebAppP2P.WebSockets;

namespace WebAppP2P.WebSockets.InternalMessages
{
    public interface IInternalMessageMediator
    {
        Task ReceiveAsync(IWebSocketConnection webSocketConnection, WebSocketMessageContract message);
        Task SendAsync(EncryptedMessage encryptedMessage);
        void Start(IWebSocketConnection webSocketConnection);
        void Close(IWebSocketConnection webSocketConnection);
    }
}