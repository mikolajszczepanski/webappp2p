using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using WebAppP2P.Core.Blockchain;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Tests.Helpers;

namespace WebAppP2P.Core.Tests.Blockchain
{
    public class BlockchainValidatorTests
    {
        [Fact]
        public void BlockchainValidator_Validate_Should_Return_True_When_Block_Is_Valid()
        {
            var bv = new BlockchainValidator();
            var bc = new BlockchainCreator();

            var blockPrevious = new Block()
            {
                BlockHash = "BLOCK_1",
                BlockHashPrevious = "BLOCK_P_1",
                Length = 20,
                Messages = new List<EncryptedMessage>(),
                Nonce = 123,
                Timestamp = 9000
            };
            var messages = new List<EncryptedMessage>();
            for (int i = 0; i < 5; i++)
            {
                messages.Add(MessageHelper.GetTestEncryptedMessage("t" + i, "c" + i));
            }

            var newBlock = bc.Create(blockPrevious, messages, (d) => 876);

            var result = bv.Validate(newBlock,blockPrevious, (d, n) => n == 876);

            Assert.True(result);
        }

        [Fact]
        public void BlockchainValidator_Validate_Should_Return_False_When_Length_Is_Incorrect()
        {
            var bv = new BlockchainValidator();
            var bc = new BlockchainCreator();

            var blockPrevious = new Block()
            {
                BlockHash = "BLOCK_1",
                BlockHashPrevious = "BLOCK_P_1",
                Length = 20,
                Messages = new List<EncryptedMessage>(),
                Nonce = 123,
                Timestamp = 9000
            };
            var messages = new List<EncryptedMessage>();
            for (int i = 0; i < 5; i++)
            {
                messages.Add(MessageHelper.GetTestEncryptedMessage("t" + i, "c" + i));
            }

            var newBlock = bc.Create(blockPrevious, messages, (d) => 876);
            newBlock.Length = 999;

            var result = bv.Validate(newBlock, blockPrevious, (d, n) => n == 876);

            Assert.False(result);
        }

        [Fact]
        public void BlockchainValidator_Validate_Should_Return_False_When_Previous_Is_Diffrent_Than_Expected()
        {
            var bv = new BlockchainValidator();
            var bc = new BlockchainCreator();

            var blockPrevious = new Block()
            {
                BlockHash = "BLOCK_1",
                BlockHashPrevious = "BLOCK_P_1",
                Length = 20,
                Messages = new List<EncryptedMessage>(),
                Nonce = 123,
                Timestamp = 9000
            };
            var messages = new List<EncryptedMessage>();
            for (int i = 0; i < 5; i++)
            {
                messages.Add(MessageHelper.GetTestEncryptedMessage("t" + i, "c" + i));
            }

            var newBlock = bc.Create(blockPrevious, messages, (d) => 876);
            newBlock.BlockHashPrevious = "BLOCK_2";

            var result = bv.Validate(newBlock, blockPrevious, (d, n) => n == 876);

            Assert.False(result);
        }

        [Fact]
        public void BlockchainValidator_Validate_Should_Return_False_When_Nonce_Is_Incorrect()
        {
            var bv = new BlockchainValidator();
            var bc = new BlockchainCreator();

            var blockPrevious = new Block()
            {
                BlockHash = "BLOCK_1",
                BlockHashPrevious = "BLOCK_P_1",
                Length = 20,
                Messages = new List<EncryptedMessage>(),
                Nonce = 123,
                Timestamp = 9000
            };
            var messages = new List<EncryptedMessage>();
            for (int i = 0; i < 5; i++)
            {
                messages.Add(MessageHelper.GetTestEncryptedMessage("t" + i, "c" + i));
            }

            var newBlock = bc.Create(blockPrevious, messages, (d) => 1000);

            var result = bv.Validate(newBlock, blockPrevious, (d, n) => n == 876);

            Assert.False(result);
        }

        [Fact]
        public void BlockchainValidator_Validate_Should_Return_False_When_New_Block_Hash_Is_Diffrent()
        {
            var bv = new BlockchainValidator();
            var bc = new BlockchainCreator();

            var blockPrevious = new Block()
            {
                BlockHash = "BLOCK_1",
                BlockHashPrevious = "BLOCK_P_1",
                Length = 20,
                Messages = new List<EncryptedMessage>(),
                Nonce = 123,
                Timestamp = 9000
            };
            var messages = new List<EncryptedMessage>();
            for (int i = 0; i < 5; i++)
            {
                messages.Add(MessageHelper.GetTestEncryptedMessage("t" + i, "c" + i));
            }

            var newBlock = bc.Create(blockPrevious, messages, (d) => 876);
            newBlock.BlockHash += "xyz";

            var result = bv.Validate(newBlock, blockPrevious, (d, n) => n == 876);

            Assert.False(result);
        }
    }
}
