using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Messages;
using Microsoft.EntityFrameworkCore;
using WebAppP2P.Core.Database.Queries;
using WebAppP2P.Core.Database;
using System.Linq;

namespace WebAppP2P.Core.Blockchain.Queries
{
    public enum GetBlockQueryType
    {
        Default,
        PreviousInMainChain
    }

    public class GetBlockQuery : IQuery<Database.Block>
    {
        public string BlockHash { get; set; }
        public GetBlockQueryType Type { get; set; } = GetBlockQueryType.Default;
    }

    public class GetBlockQueryHandler : IQueryHandler<GetBlockQuery, Database.Block>
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public GetBlockQueryHandler(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        public Database.Block Handle(GetBlockQuery query)
        {
            return _applicationDatabase.BlockChain
                .Include(b => b.BlockMessages)
                .ThenInclude(b => b.EncryptedMessageStore)
                .SingleOrDefault(
                    b => ( query.Type == GetBlockQueryType.PreviousInMainChain && b.BlockHashPrevious == query.BlockHash && b.IsInMainChain == true ) ||
                         ( query.Type == GetBlockQueryType.Default && b.BlockHash == query.BlockHash)
                    );
        }
    }
}
