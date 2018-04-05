using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Nodes
{
    public class NodesRepository : INodesRepository
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public NodesRepository(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
            Console.WriteLine("\tNodesRepository ctor -> {0}", applicationDatabase.GetHashCode());
        }

        public async Task AddNodesAsync(IEnumerable<string> nodesUrl)
        {
            foreach (var url in nodesUrl)
            {
                await AddNodeAsync(url);
            }
        }

        public async Task<bool> AddNodeAsync(string url)
        {
            if (_applicationDatabase.Nodes.Where(n => n.Url == url).ToList().Count() > 0)
            {
                return false;
            }
            _applicationDatabase.Nodes.Add(new Database.Node()
            {
                Url = url.EndsWith("/") ? url : url + "/"
            });
            return await _applicationDatabase.SaveChangesAsync() > 0;
        }

        public IEnumerable<string> GetNodes(NodesRepositoryFilter filter)
        {
            return GetNodesFromFilter(filter)
                .Select(n => n.Url)
                .ToList();
        }

        public IEnumerable<string> GetNodes(NodesRepositoryFilter filter,int max)
        {
            return GetNodesFromFilter(filter)
                 .Take(max)
                 .Select(n => n.Url)
                 .ToList();
        }

        public async Task RegisterNodeStatisticAsync(string url, long delayInSeconds, bool isSuccess)
        {
            var node = _applicationDatabase.Nodes.Where(n => n.Url == url).SingleOrDefault();
            if (node == null)
            {
                await AddNodeAsync(url);
            }
            _applicationDatabase.Statistics.Add(new Database.NodeStatistics()
            {
                IsSuccess = isSuccess,
                Latency = (int)delayInSeconds,
                NodeId = node.Id,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
            await _applicationDatabase.SaveChangesAsync();
        }

        private IQueryable<Database.Node> GetNodesFromFilter(NodesRepositoryFilter filter)
        {
            if (filter == NodesRepositoryFilter.All)
            {
                return _applicationDatabase.Nodes;
            }
            else if (filter == NodesRepositoryFilter.OnlyActive)
            {
                return _applicationDatabase.Nodes
                    .Where(n => n.IsActive == true);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
