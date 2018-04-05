using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Messages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebAppP2P.Core.Database.Queries;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Blockchain.Queries
{
    public class GetHeadBlockQuery : IQuery<Database.Block>
    {
    }

    public class GetHeadBlockQueryHandler : IQueryHandler<GetHeadBlockQuery, Database.Block>
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public GetHeadBlockQueryHandler(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        public Database.Block Handle(GetHeadBlockQuery query)
        {
            return _applicationDatabase.BlockChain
                .Include(b => b.BlockMessages)
                .ThenInclude(b => b.EncryptedMessageStore)
                .OrderByDescending(b => b.Length)
                .First();
        }
    }
}
