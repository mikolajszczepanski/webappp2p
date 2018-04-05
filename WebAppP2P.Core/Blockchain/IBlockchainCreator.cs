using System;
using System.Collections.Generic;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Messages;

namespace WebAppP2P.Core.Blockchain
{
    public interface IBlockchainCreator
    {
        Block Create(Block headBlock, IEnumerable<EncryptedMessage> messages, Func<string, ulong> getNonce);
    }
}