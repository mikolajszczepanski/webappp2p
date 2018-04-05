using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppP2P.Dto;

namespace WebAppP2P.WebSockets.InternalMessages
{

    public class InternalMessageJsonDeserializer
    {
        public WebSocketMessageContract Deserialize(string json)
        {
            var message = JsonConvert.DeserializeObject<WebSocketMessageContractType>(json);
            if (message == null)
            {
                throw new ArgumentException("Invalid message");
            }
            var deserializedMessage = new WebSocketMessageContract()
            {
                Type = message.Type
            };
            switch (message.Type){
                case WebSocketMessageRequestTypes.CONFIG:
                    var tempConfigMessage = JsonConvert.DeserializeObject<WebSocketMessageContract<ConfigMessage>>(json);
                    deserializedMessage.Data = tempConfigMessage.Data;
                    break;
                case WebSocketMessageRequestTypes.MESSAGE:
                    var tempClientInternalMessage = JsonConvert.DeserializeObject<WebSocketMessageContract<ClientInternalMessage>>(json);
                    deserializedMessage.Data = tempClientInternalMessage.Data;
                    break;
                case WebSocketMessageRequestTypes.SYNCHRONIZATION:
                    var tempClientSynchronization = JsonConvert.DeserializeObject<WebSocketMessageContract<ClientSynchronizationMessage>>(json);
                    deserializedMessage.Data = tempClientSynchronization.Data;
                    break;
                default:
                    throw new ArgumentException(nameof(message.Type));
            }
            return deserializedMessage;
        }
    }
}
