using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using WebAppP2P.Core.Tests.Helpers;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Blockchain.Queries;
using WebAppP2P.Core.Database.Queries;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebAppP2P.Core.Tests.Blockchain.Queries
{
    public class MarkMainChainQueryTests
    {
        private static ApplicationDatabase GetDatabase_1()
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
                    Length = 3
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

        private static ApplicationDatabase GetDatabase_2()
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
                    BlockHashPrevious = "BLOCK_3",
                    BlockHash = "BLOCK_4",
                    Length = 4
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_3",
                    BlockHash = "BLOCK_5",
                    Length = 4,
                    IsInMainChain = true
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_5",
                    BlockHash = "BLOCK_6",
                    Length = 5,
                    IsInMainChain = true
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_5",
                    BlockHash = "BLOCK_7",
                    Length = 5
                });
                a.SaveChanges();
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_7",
                    BlockHash = "BLOCK_8",
                    Length = 6
                });
                a.SaveChanges();
            });
        }

        private static ApplicationDatabase GetDatabase_3()
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
            });
        }

        [Fact]
        public void MarkMainChainQuery_Should_Mark_Longest_Chain_Use_Case_No_Longer_Branches()
        {
            var db = GetDatabase_1();
            var query = new MarkMainChainQueryHandler(db, new GetHeadBlockQueryHandlerMock(db, "BLOCK_3"));

            var result = query.Handle(new MarkMainChainQuery()
            {
                IsLongRunning = false
            });

            Assert.True(result);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "GENESIS" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_1" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_2" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_3" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_4" && b.IsInMainChain == false) == 1);
        }

        [Fact]
        public void MarkMainChainQuery_Should_Mark_Longest_Chain_Use_Case_Longer_Branch()
        {
            var db = GetDatabase_2();
            var query = new MarkMainChainQueryHandler(db, new GetHeadBlockQueryHandlerMock(db, "BLOCK_8"));

            var result = query.Handle(new MarkMainChainQuery()
            {
                IsLongRunning = false
            });

            Assert.True(result);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "GENESIS" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_1" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_2" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_3" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_4" && b.IsInMainChain == false) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_5" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_6" && b.IsInMainChain == false) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_7" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_8" && b.IsInMainChain == true) == 1);

        }

        [Fact]
        public void MarkMainChainQuery_Should_Mark_First_Block_When_Blockchain_Has_Lenght_One()
        {
            var db = GetDatabase_3();
            var query = new MarkMainChainQueryHandler(db, new GetHeadBlockQueryHandlerMock(db, "BLOCK_1"));

            var result = query.Handle(new MarkMainChainQuery()
            {
                IsLongRunning = false
            });

            Assert.True(result);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "GENESIS" && b.IsInMainChain == true) == 1);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "BLOCK_1" && b.IsInMainChain == true) == 1);
        }

        class GetHeadBlockQueryHandlerMock : IQueryHandler<GetHeadBlockQuery, Database.Block>
        {
            private readonly ApplicationDatabase _applicationDatabase;
            private readonly string _headBlockHash;

            public GetHeadBlockQueryHandlerMock(ApplicationDatabase applicationDatabase, string headBlockHash)
            {
                _applicationDatabase = applicationDatabase;
                _headBlockHash = headBlockHash;
            }

            public Block Handle(GetHeadBlockQuery query)
            {
                return _applicationDatabase.BlockChain.Single(b => b.BlockHash == _headBlockHash);
            }
        }
    }
}
