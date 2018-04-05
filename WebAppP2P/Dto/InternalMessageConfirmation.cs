using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto
{
    public class InternalMessageConfirmation
    {
        public long CorrelationId { get; set; }
        public string Id { get; set; }
        public long Timestamp { get; set; }
        public string DateTime { get; set; }
    }
}
