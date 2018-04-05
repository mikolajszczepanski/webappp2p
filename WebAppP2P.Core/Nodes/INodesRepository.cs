using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAppP2P.Core.Nodes
{
    public enum NodesRepositoryFilter
    {
        All,
        OnlyActive
    }

    public interface INodesRepository
    {
        Task AddNodesAsync(IEnumerable<string> nodesUrl);
        Task<bool> AddNodeAsync(string url);
        IEnumerable<string> GetNodes(NodesRepositoryFilter filter);
        IEnumerable<string> GetNodes(NodesRepositoryFilter filter,int max);
        Task RegisterNodeStatisticAsync(string url, long delayInSeconds, bool isSuccess);
    }
}