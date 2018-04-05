using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Database.Queries;

namespace WebAppP2P.Core.Blockchain.Queries
{
    public class AddBlockQuery : IQuery<bool>
    {
        public Block NewBlock { get; set; }
        public bool IsLongRunning { get; set; } = true;
    }

    public class AddBlockQueryHandler : IQueryHandler<AddBlockQuery, bool>
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public AddBlockQueryHandler(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        public bool Handle(AddBlockQuery query)
        {
            if (query.IsLongRunning)
            {
                _applicationDatabase.Database.SetCommandTimeout(new TimeSpan(1, 0, 0));
            }
            try
            {
                var dbNewBlock = new Database.Block()
                {
                    BlockHash = query.NewBlock.BlockHash,
                    BlockHashPrevious = query.NewBlock.BlockHashPrevious,
                    Length = query.NewBlock.Length,
                    Nonce = query.NewBlock.Nonce,
                    Timestamp = query.NewBlock.Timestamp
                };
                _applicationDatabase.BlockChain.AddAsync(dbNewBlock);
                _applicationDatabase.SaveChanges();

                foreach (var bm in query.NewBlock.Messages)
                {
                    var msg = _applicationDatabase.Messages.Single(e => e.Id == bm.Id);
                    _applicationDatabase.BlockMessages.Add(new BlockMessages()
                    {
                        StoreId = msg.StoreId,
                        BlockHash = dbNewBlock.BlockHash
                    });
                }
                _applicationDatabase.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("AddBlockQueryHandler: {0} \n{1}",ex.Message,ex.StackTrace);
                return false;
            }
        }
    }
}
