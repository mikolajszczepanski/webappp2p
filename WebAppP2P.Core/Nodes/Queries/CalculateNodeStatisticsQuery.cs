using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAppP2P.Core.Database.Queries;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Nodes.Queries
{
    public class CalculateNodeStatisticsQuery : IQuery
    {
        public int MaxActives { get; set; }
        public long MinTimestamp { get; set; }
        public bool IsLongRunning { get; set; } = true;
    }

    public class CalculateNodeStatisticsQueryHandler : IQueryHandler<CalculateNodeStatisticsQuery>
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public CalculateNodeStatisticsQueryHandler(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        public void Handle(CalculateNodeStatisticsQuery options)
        {
            if (options.IsLongRunning)
            {
                _applicationDatabase.Database.SetCommandTimeout(new TimeSpan(0, 10, 0));
            }
            Console.WriteLine("\tCalculateNodeStatisticsQueryHandler {0}", DateTime.Now);
            var nodesWithLatency = new List<Tuple<int,Node>>();

            foreach (var node in _applicationDatabase.Nodes)
            {
                var sum = _applicationDatabase.Statistics
                    .Where(s => s.NodeId == node.Id && s.Timestamp > options.MinTimestamp)
                    .Select(s => s.IsSuccess ? 1 : 0)
                    .Sum();
                var count = _applicationDatabase.Statistics
                    .Where(s => s.NodeId == node.Id && s.Timestamp > options.MinTimestamp)
                    .Count();
                var latency = _applicationDatabase.Statistics
                    .Where(s => s.NodeId == node.Id && s.Timestamp > options.MinTimestamp)
                    .Select(s => s.Latency)
                    .Sum();
                if (sum >= (count - sum))
                {
                    nodesWithLatency.Add(new Tuple<int, Node>(latency,node));
                }
                else
                {
                    node.IsActive = false;
                    _applicationDatabase.SaveChanges();
                }
            }
            foreach (var node in nodesWithLatency
                .OrderBy(t => t.Item1)
                .Take(options.MaxActives)
                .Select(t => t.Item2)
                )
            {
                node.IsActive = true;
                node.LastActiveTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                _applicationDatabase.SaveChanges();
            }
            foreach (var node in nodesWithLatency
                .OrderBy(t => t.Item1)
                .Skip(options.MaxActives)
                .Select(t => t.Item2)
                )
            {
                node.IsActive = false;
                _applicationDatabase.SaveChanges();
            }

        }
    }
}
