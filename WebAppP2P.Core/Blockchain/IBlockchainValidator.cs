using System;

namespace WebAppP2P.Core.Blockchain
{
    public interface IBlockchainValidator
    {
        bool Validate(Block blockToValidate, Block previousBlock, Func<string, ulong, bool> validateNonce);
    }
}