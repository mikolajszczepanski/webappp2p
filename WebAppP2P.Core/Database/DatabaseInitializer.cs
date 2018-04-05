using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WebAppP2P.Core.Database
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public DatabaseInitializer(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        public void Seed()
        {
            if( _applicationDatabase.BlockChain.Where(b => b.BlockHash == BlockchainConsensus.GenesisBlock.BlockHash).Count() > 0)
            {
                return;
            }

            _applicationDatabase.BlockChain.Add(
                BlockchainConsensus.GenesisBlock
                );
            _applicationDatabase.SaveChanges();
        }
        
    }
}
