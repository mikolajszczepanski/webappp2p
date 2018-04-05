using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppP2P.Services;

namespace WebAppP2P.WebSockets.InternalMessages
{
    public static class InternalMessageExtensions
    {
        public static IServiceCollection AddInternalMessagesServices(this IServiceCollection services)
        {
            services.AddTransient<IWebSocketConnectionHandler, InternalMessageWebsocketConnectionHandler>();
            services.AddTransient<IInternalMessageMediator, InternalMessageMediator>();
            services.AddTransient<IPeerCommunicationService, PeerCommunicationService>();
            services.AddTransient<IInternalMessageSender, InternalMessageSender>();
            return services;
        }
    }
}
