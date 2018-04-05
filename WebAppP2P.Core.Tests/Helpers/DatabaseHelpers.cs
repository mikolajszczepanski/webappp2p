using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Tests.Helpers
{
    public static class DatabaseHelpers
    {
        /// <summary>
        /// Return empty database with genesis block
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static ApplicationDatabase GetDatabase(Action<ApplicationDatabase> seed)
        {
            var options = new DbContextOptionsBuilder<ApplicationDatabase>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                      .Options;
            var context = new ApplicationDatabase(options);
            context.BlockChain.Add(BlockchainConsensus.GenesisBlock);
            context.SaveChanges();

            seed(context);
            return context;
        }
    }
}
