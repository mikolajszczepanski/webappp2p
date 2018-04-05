using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Blockchain.Queries;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;

namespace WebAppP2P.Core.Tests.Blockchain.Queries
{
    public class GetHeadBlockQueryTests
    {
        [Fact]
        public void GetHeadBlockQuery_Should_Return_Genesis_When_No_Other_Exists()
        {
            var dbContext = DatabaseHelpers.GetDatabase(a => { });
            var query = new GetHeadBlockQueryHandler(dbContext);

            var block = query.Handle(new GetHeadBlockQuery()
            {
                
            });

            Assert.True(block.BlockHash == BlockchainConsensus.GenesisBlock.BlockHash);
            Assert.True(block.BlockHashPrevious == BlockchainConsensus.GenesisBlock.BlockHashPrevious);
            Assert.True(block.Timestamp == BlockchainConsensus.GenesisBlock.Timestamp);
            Assert.True(block.Nonce == BlockchainConsensus.GenesisBlock.Nonce);
            Assert.True(block.BlockMessages == BlockchainConsensus.GenesisBlock.BlockMessages);
        }

        [Fact]
        public void GetHeadBlockQuery_Should_Return_Block_With_The_Longest_Length_When_More_Exists_And_Without_Branches()
        {
            var dbContext = DatabaseHelpers.GetDatabase(a => {
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "GENESIS",
                    BlockHash = "BLOCK_1",
                    Length = 1
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_2",
                    Length = 2
                });
                a.SaveChanges();
            });
            var query = new GetHeadBlockQueryHandler(dbContext);

            var block = query.Handle(new GetHeadBlockQuery()
            {

            });

            Assert.True(block.BlockHash == "BLOCK_2");
            Assert.True(block.Length == 2);
            Assert.True(block.BlockHashPrevious == "BLOCK_1");
        }

        [Fact]
        public void GetHeadBlockQuery_Should_Return_Block_With_The_Longest_Length_When_More_Exists_Longest()
        {
            var dbContext = DatabaseHelpers.GetDatabase(a => {
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "GENESIS",
                    BlockHash = "BLOCK_1",
                    Length = 1
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_2",
                    Length = 2
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_3",
                    Length = 2
                });
                a.SaveChanges();
            });
            var query = new GetHeadBlockQueryHandler(dbContext);

            var block = query.Handle(new GetHeadBlockQuery()
            {

            });

            Assert.True(block.BlockHash == "BLOCK_2" || block.BlockHash == "BLOCK_3");
            Assert.True(block.Length == 2);
            Assert.True(block.BlockHashPrevious == "BLOCK_1");
        }

        [Fact]
        public void GetHeadBlockQuery_Should_Return_Block_With_The_Longest_Length_When_More_Exists_And_With_Branches()
        {
            var dbContext = DatabaseHelpers.GetDatabase(a => {
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "GENESIS",
                    BlockHash = "BLOCK_1",
                    Length = 1
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_2",
                    Length = 2
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_2",
                    BlockHash = "BLOCK_3",
                    Length = 3
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_5",
                    Length = 2
                });
                a.SaveChanges();
            });
            var query = new GetHeadBlockQueryHandler(dbContext);

            var block = query.Handle(new GetHeadBlockQuery()
            {

            });

            Assert.True(block.BlockHash == "BLOCK_3");
            Assert.True(block.Length == 3);
            Assert.True(block.BlockHashPrevious == "BLOCK_2");
        }

    }
}
