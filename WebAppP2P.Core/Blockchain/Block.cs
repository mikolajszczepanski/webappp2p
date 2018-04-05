using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;
using System.Linq;

namespace WebAppP2P.Core.Blockchain
{
    public class Block
    {
        public string BlockHash { get; set; }
        public string BlockHashPrevious { get; set; }
        public long Timestamp { get; set; }
        public ulong Nonce { get; set; }
        public uint Length { get; set; }
        public IEnumerable<EncryptedMessage> Messages { get; set; }

        public Block()
        {

        }

        public Block(Database.Block block)
        {
            BlockHash = block.BlockHash;
            BlockHashPrevious = string.IsNullOrEmpty(block.BlockHashPrevious) ? string.Empty : block.BlockHashPrevious;
            Timestamp = block.Timestamp;
            Nonce = block.Nonce;
            Length = block.Length;
            Messages = block.BlockMessages
                .Where(b => b.EncryptedMessageStore != null)
                .Select(bm => new EncryptedMessage(bm.EncryptedMessageStore));
        }

        public Block(Database.Block block, IEnumerable<EncryptedMessage> messages)
        {
            BlockHash = block.BlockHash;
            BlockHashPrevious = string.IsNullOrEmpty(block.BlockHashPrevious) ? string.Empty : block.BlockHashPrevious;
            Timestamp = block.Timestamp;
            Nonce = block.Nonce;
            Length = block.Length;
            Messages = block.BlockMessages
                .Where(b => b.EncryptedMessageStore != null)
                .Select(bm => new EncryptedMessage(bm.EncryptedMessageStore));
        }
    }
}
