using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P
{
    public class NodeOptions
    {
        public IEnumerable<string> NodesList { get; set; }
        public TracerOptions Tracer { get; set; }
        public BlockchainOptions Blockchain { get; set; }
        public string Self { get; set; }
    }

    public class TracerOptions
    {
        public int MaxNodesActive { get; set; }
        public int JobInterval { get; set; }
        public int ClearStatistics { get; set; }
        public int StatisticsConsider { get; set; }
    }

    public class BlockchainOptions
    {
        public int JobInterval { get; set; }
    }
}
