using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Database.Queries;

namespace WebAppP2P.Core.Nodes.Queries
{

    public class ClearNodeStatisticsQuery : IQuery
    {
        public long MinTimestamp { get; set; }
    }

    public class ClearNodeStatisticsQueryHandler : IQueryHandler<ClearNodeStatisticsQuery>
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public ClearNodeStatisticsQueryHandler(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        public void Handle(ClearNodeStatisticsQuery options)
        {
            var toDelete = _applicationDatabase.Statistics
                .Where(s => s.Timestamp < options.MinTimestamp);
            _applicationDatabase.Statistics.RemoveRange(toDelete);
            _applicationDatabase.SaveChanges();
        }
    }
}
