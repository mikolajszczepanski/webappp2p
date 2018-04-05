using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppP2P.WebSockets
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketConnectionHandler _handler;
        private readonly string _path;

        public WebSocketMiddleware(RequestDelegate requestDelegate,
            IWebSocketConnectionHandler handler,
            string path
            )
        {
            _next = requestDelegate;
            _handler = handler;
            _path = path;
        }

        public async Task Invoke(HttpContext context)
        {
            if (Regex.IsMatch(context.Request.Path.Value, _path))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Receive(webSocket);
                }
            }
            await _next(context);
        }

        private async Task Receive(WebSocket webSocket)
        {
            if(webSocket == null)
            {
                return;
            }
            IWebSocketConnection webSocketConnection = new WebSocketConnection(webSocket, Guid.NewGuid());
            _handler.OnStart(webSocketConnection);
            while (webSocket.State == WebSocketState.Open)
            {
                await _handler.OnReceiveAsync(webSocketConnection);
            }
            await _handler.OnCloseAsync(webSocketConnection);
        }

    }
}
