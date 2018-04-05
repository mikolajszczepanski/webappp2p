using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Tests.Helpers;
using WebAppP2P.Core.Blockchain.Queries;
using System.Threading.Tasks;
using WebAppP2P.Core.Messages;

namespace WebAppP2P.Core.Tests.Blockchain.Queries
{
    public class AddBlockQueryTests
    {
        private static ApplicationDatabase GetDatabase()
        {
            return DatabaseHelpers.GetDatabase(a => {
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "GENESIS",
                    BlockHash = "BLOCK_1",
                    Length = 1
                });
                a.SaveChanges();

                a.Messages.Add(new EncryptedMessageStore()
                {
                    StoreId = 1,
                    Id = "MSG1",
                    Content = "NO IN BLOCK",
                    Title = "test",
                    From = "fr",
                    To = "to",
                    FromKey = "fk",
                    ToKey = "tk",
                    Nonce = 1,
                    IV = "iv"
                });
                a.Messages.Add(new EncryptedMessageStore()
                {
                    StoreId = 2,
                    Id = "MSG2",
                    Content = "IN BLOCK",
                    Title = "test",
                    From = "fr",
                    To = "to",
                    FromKey = "fk",
                    ToKey = "tk",
                    Nonce = 1,
                    IV = "iv"
                });

                a.BlockMessages.Add(new BlockMessages()
                {
                    StoreId = 2,
                    BlockHash = "BLOCK_1"
                });
            });
        }

        [Fact]
        public void AddBlockQuery_Should_Return_True_When_Successfully_Added_Block()
        {
            var db = GetDatabase();
            var query = new AddBlockQueryHandler(db);

            var result = query.Handle(new AddBlockQuery()
            {
                NewBlock = new Core.Blockchain.Block()
                {
                    BlockHash = "NEW_BLOCK",
                    BlockHashPrevious = "BLOCK_1",
                    Length = 2,
                    Nonce = 1,
                    Timestamp = 2000,
                    Messages = new List<EncryptedMessage>()
                    {
                        new EncryptedMessage()
                        {
                            Id = "MSG1",
                            Content = "NO IN BLOCK",
                            Title = "test",
                            From = "fr",
                            To = "to",
                            FromKey = "fk",
                            ToKey = "tk",
                            Nonce = 1,
                            IV = "iv"
                        }
                    }
                },
                IsLongRunning = false
            });

            Assert.True(result);
            Assert.True(db.BlockChain.Count(b => b.BlockHash == "NEW_BLOCK") == 1);
            Assert.True(db.BlockMessages.Count(bm => bm.StoreId == 1 && bm.BlockHash == "NEW_BLOCK") == 1);
        }

        [Fact]
        public void AddBlockQuery_Should_Return_False_When_Message_From_New_Block_Is_Not_In_Db()
        {
            var db = GetDatabase();
            var query = new AddBlockQueryHandler(db);

            var result = query.Handle(new AddBlockQuery()
            {
                NewBlock = new Core.Blockchain.Block()
                {
                    BlockHash = "NEW_BLOCK",
                    BlockHashPrevious = "BLOCK_1",
                    Length = 2,
                    Nonce = 1,
                    Timestamp = 2000,
                    Messages = new List<EncryptedMessage>()
                    {
                        new EncryptedMessage()
                        {
                            Id = "MSG3",
                            Content = "NO IN BLOCK",
                            Title = "test",
                            From = "fr",
                            To = "to",
                            FromKey = "fk",
                            ToKey = "tk",
                            Nonce = 1,
                            IV = "iv"
                        }
                    }
                },
                IsLongRunning = false
            });

            Assert.False(result);
        }
    }
}
