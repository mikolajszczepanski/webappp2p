using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Blockchain;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;
using System.Linq;

namespace WebAppP2P.Core.Tests.Blockchain
{
    public class BlockchainCreatorTests
    {
        [Fact]
        public void BlockchainCreator_Create_Should_Return_Valid_New_Block()
        {
            var bc = new BlockchainCreator();
            var headBlock = new Block()
            {
                BlockHash = "BLOCK_TEST",
                BlockHashPrevious = "BLOCK_TEST2",
                Length = 10,
                Messages = new List<EncryptedMessage>(),
                Nonce = 321,
                Timestamp = 1000
            };
            var messages = new List<EncryptedMessage>();
            for (int i = 0; i < 5; i++)
            {
                messages.Add(MessageHelper.GetTestEncryptedMessage("t" + i, "c" + i));
            }

            var newBlock = bc.Create(headBlock, messages, (d) => 876);

            Assert.NotNull(newBlock.BlockHash);
            Assert.True(newBlock.Length == 11);
            Assert.True(newBlock.Timestamp > headBlock.Timestamp);
            Assert.True(newBlock.Messages.Count() == 5);
            Assert.True(newBlock.Nonce == 876);
        }
    }
}
