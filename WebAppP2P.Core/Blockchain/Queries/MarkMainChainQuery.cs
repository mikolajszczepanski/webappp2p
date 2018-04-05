using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Database.Queries;
using System.Linq;

namespace WebAppP2P.Core.Blockchain.Queries
{
    public class MarkMainChainQuery : IQuery<bool>
    {
        public bool IsLongRunning { get; set; } = true;
    }

    public class MarkMainChainQueryHandler : IQueryHandler<MarkMainChainQuery, bool>
    {
        private readonly IQueryHandler<GetHeadBlockQuery, Database.Block> _queryHandlerGetHeadBlock;
        private readonly ApplicationDatabase _applicationDatabase;


        public MarkMainChainQueryHandler(ApplicationDatabase applicationDatabase,
                                         IQueryHandler<GetHeadBlockQuery, Database.Block> queryHandlerGetHeadBlock
            )
        {
            _applicationDatabase = applicationDatabase;
            _queryHandlerGetHeadBlock = queryHandlerGetHeadBlock;
        }

        public bool Handle(MarkMainChainQuery query)
        {
            if (query.IsLongRunning)
            {
                _applicationDatabase.Database.SetCommandTimeout(new TimeSpan(0, 10, 0));
            }
            using (var transaction = _applicationDatabase.Database.BeginTransaction()) {
                try
                {
                    _applicationDatabase.BlockChain
                        .Where(b => b.IsInMainChain && b.BlockHash != BlockchainConsensus.GenesisBlock.BlockHash)
                        .ToList()
                        .ForEach(b => b.IsInMainChain = false);
                    _applicationDatabase.SaveChangesAsync();

                    var headBlock = _queryHandlerGetHeadBlock.Handle(new GetHeadBlockQuery(){});
                    if (headBlock.IsInMainChain == false)
                    {
                        _applicationDatabase.BlockChain.Update(headBlock);
                        headBlock.IsInMainChain = true;
                        _applicationDatabase.SaveChanges();
                    }
                    string blockHashPrevious = headBlock.BlockHashPrevious;
                    while (!string.IsNullOrEmpty(blockHashPrevious) && blockHashPrevious != BlockchainConsensus.GenesisBlock.BlockHash)
                    {
                        var block = _applicationDatabase.BlockChain.Single(b => b.BlockHash == blockHashPrevious);
                        blockHashPrevious = block.BlockHashPrevious;
                        block.IsInMainChain = true;
                        _applicationDatabase.SaveChanges();

                    } 
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
