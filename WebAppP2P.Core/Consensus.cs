using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core
{
    public static class MessagesConsensus
    {
        public const int MAX_TITLE_BYTES = 96;
        public const int MAX_CONTENT_BYTES = 1024;
    }

    public static class BlockchainConsensus
    {
        public static readonly Block GenesisBlock = new Block()
        {
            BlockHashPrevious = null,
            BlockHash = "GENESIS",
            Nonce = 0,
            Timestamp = 1511088555,
            IsInMainChain = true
        };

        public const int MIN_BLOCK_MESSAGES = 1;
    }

}
