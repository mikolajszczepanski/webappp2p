using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto
{
    public class WebSocketMessageContractType
    {
        public string Type { get; set; }
    }

    public class WebSocketMessageContract
    {
        public string Type { get; set; }
        public object Data { get; set; }
    }

    public class WebSocketMessageContract<T>
    {
        public string Type { get; set; }
        public T Data { get; set; }
    }
}
