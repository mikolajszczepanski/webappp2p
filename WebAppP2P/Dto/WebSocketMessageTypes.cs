using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto
{
    public static class WebSocketMessageRequestTypes
    {
        public const string CONFIG = "WEBSOCKET_CONFIG_MESSAGE";
        public const string MESSAGE = "WEBSOCKET_MESSAGE";
        public const string SYNCHRONIZATION = "WEBSOCKET_MESSAGE_SYNCHRONIZATION";

    }

    public static class WebSocketMessageResponseTypes
    {
        public const string INTERNAL_MESSAGE = "WEBSOCKET_MESSAGE";
        public const string INTERNAL_MESSAGE_CONFIRMATION = "WEBSOCKET_MESSAGE_CONFIRMATION";
        public const string INTERNAL_MESSAGE_ERROR = "WEBSOCKET_MESSAGE_ERROR";
        public const string END_OF_SYNCHRONIZATION = "WEBSOCKET_END_OF_SYNCHRONIZATION";
        public const string ERROR = "WEBSOCKET_ERROR";
    }
}
