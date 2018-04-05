using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto
{
    public class InternalMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long Timestamp { get; set; }
        public string Id { get; set; }
        public string DateTime { get; set; }
    }
}
