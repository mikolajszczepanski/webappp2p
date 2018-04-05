using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto
{
    public class ClientInternalMessage
    {
        public string To { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long? CorrelationId { get; set; }
    }
}
