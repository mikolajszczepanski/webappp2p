using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Blockchain.Queries;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;

namespace WebAppP2P.Core.Tests.Blockchain.Queries
{
    public class GetBlockQueryTests
    {
        private static ApplicationDatabase GetDatabase()
        {
            return DatabaseHelpers.GetDatabase(a => {
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "GENESIS",
                    BlockHash = "BLOCK_1",
                    Length = 1,
                    IsInMainChain = true
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_2",
                    Length = 2,
                    IsInMainChain = true
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_2",
                    BlockHash = "BLOCK_3",
                    Length = 3,
                    IsInMainChain = true
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_4",
                    Length = 2
                });
                a.SaveChanges();
            });
        }

        [Fact]
        public void GetBlockQuery_Should_Return_Proper_Block_When_BlockHash_Is_Defined()
        {
            var dbContext = GetDatabase();
            var query = new GetBlockQueryHandler(dbContext);

            var block = query.Handle(new GetBlockQuery()
            {
                BlockHash = "BLOCK_4"
            });

            Assert.True(block.BlockHash == "BLOCK_4");
            Assert.True(block.BlockHashPrevious == "BLOCK_1");
            Assert.True(block.Length == 2);
        }

        [Fact]
        public void GetBlockQuery_Should_Return_Null_When_More_Do_Not_Found_Block()
        {
            var dbContext = GetDatabase();
            var query = new GetBlockQueryHandler(dbContext);

            var block = query.Handle(new GetBlockQuery()
            {
                BlockHash = "BLOCK_4",
                Type = GetBlockQueryType.PreviousInMainChain
            });

            Assert.Null(block);
        }

        [Fact]
        public void GetBlockQuery_Should_Return_Proper_Block_When_BlockHash_And_Previous_Are_Defined()
        {
            var dbContext = GetDatabase();
            var query = new GetBlockQueryHandler(dbContext);

            var block = query.Handle(new GetBlockQuery()
            {
                BlockHash = "BLOCK_2",
                Type = GetBlockQueryType.PreviousInMainChain
            });

            Assert.True(block.BlockHash == "BLOCK_3");
            Assert.True(block.BlockHashPrevious == "BLOCK_2");
            Assert.True(block.Length == 3);
            Assert.True(block.IsInMainChain);
        }

        [Fact]
        public void GetBlockQuery_Should_Return_Proper_Block_InMainChain_When_BlockHash_And_Previous_Are_Defined_With_Branch()
        {
            var dbContext = GetDatabase();
            var query = new GetBlockQueryHandler(dbContext);

            var block = query.Handle(new GetBlockQuery()
            {
                BlockHash = "BLOCK_1",
                Type = GetBlockQueryType.PreviousInMainChain
            });

            Assert.True(block.BlockHash == "BLOCK_2");
            Assert.True(block.BlockHashPrevious == "BLOCK_1");
            Assert.True(block.Length == 2);
            Assert.True(block.IsInMainChain);
        }
    }
}
