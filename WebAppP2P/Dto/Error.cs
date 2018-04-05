using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Dto
{
    public class Error
    {
        public string Description { get; set; }
        public Exception Exception { get; set; }
    }
}
