using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto
{
    public class InternalMessageError
    {
        public long CorrelationId { get; set; }
        public string Description { get; set; }
    }
}
