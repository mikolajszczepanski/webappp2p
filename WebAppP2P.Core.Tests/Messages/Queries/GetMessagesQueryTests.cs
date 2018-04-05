using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Blockchain.Queries;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Messages.Queries;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;
using System.Linq;

namespace WebAppP2P.Core.Tests.Messages.Queries
{
    public class GetMessagesQueryTests
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
                a.BlockChain.Add(new Block()
                {
                    BlockHashPrevious = "BLOCK_1",
                    BlockHash = "BLOCK_2",
                    Length = 2
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
                a.SaveChanges();

                a.BlockMessages.Add(new BlockMessages()
                {
                    BlockHash = "BLOCK_1",
                    StoreId = 2
                });
                a.SaveChanges();
            });
        }

        [Fact]
        public void GetMessagesQuery_Should_Return_All_When_Type_All()
        {
            var dbContext = GetDatabase();
            var query = new GetMessagesQueryHandler(dbContext);

            var messages = query.Handle(new GetMessagesQuery()
            {
                Type = EncryptedMessageType.All
            });

            Assert.True(messages.Count() == 2);
            Assert.True(messages.Count(m => m.StoreId == 1) == 1);
            Assert.True(messages.Count(m => m.StoreId == 2) == 1);
        }

        [Fact]
        public void GetMessagesQuery_Should_Return_Messages_In_Blockchain_When_Type_OnlyInBlockchain()
        {
            var dbContext = GetDatabase();
            var query = new GetMessagesQueryHandler(dbContext);

            var messages = query.Handle(new GetMessagesQuery()
            {
                Type = EncryptedMessageType.OnlyInBlockchain
            });

            Assert.True(messages.Count() == 1);
            Assert.True(messages.Count(m => m.StoreId == 2) == 1);
            Assert.True(messages.Count(m => m.Id == "MSG2") == 1);
        }

        [Fact]
        public void GetMessagesQuery_Should_Return_Messages_Not_In_Blockchain_When_Type_OnlyOuterBlockchain()
        {
            var dbContext = GetDatabase();
            var query = new GetMessagesQueryHandler(dbContext);

            var messages = query.Handle(new GetMessagesQuery()
            {
                Type = EncryptedMessageType.OnlyOuterBlockchain
            });

            Assert.True(messages.Count() == 1);
            Assert.True(messages.Count(m => m.StoreId == 1) == 1);
            Assert.True(messages.Count(m => m.Id == "MSG1") == 1);
        }


    }
}
