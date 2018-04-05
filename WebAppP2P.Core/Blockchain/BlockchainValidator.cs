using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;
using System.Linq;
using System.Security.Cryptography;

namespace WebAppP2P.Core.Blockchain
{

    public class BlockchainValidator : IBlockchainValidator
    {
        public bool Validate(Block blockToValidate, Block previousBlock, Func<string,ulong,bool> validateNonce)
        {
            if(previousBlock.BlockHash != blockToValidate.BlockHashPrevious)
            {
                return false;
            }
            if(blockToValidate.Messages.Count() == 0)
            {
                return false;
            }
            if((previousBlock.Length + 1) != blockToValidate.Length)
            {
                return false;
            }

            var sortedMessages = blockToValidate.Messages.ToList();
            sortedMessages.Sort((m1, m2) => m1.Id.CompareTo(m2.Id));

            StringBuilder sb = new StringBuilder();
            foreach (var msg in sortedMessages)
            {
                sb.Append(msg.Id);
            }
            sb.Append(blockToValidate.Length);
            sb.Append(blockToValidate.Timestamp);
            sb.Append(blockToValidate.BlockHashPrevious);
            var data = sb.ToString();

            if (!validateNonce(data, blockToValidate.Nonce))
            {
                return false;
            }

            using (var sha = new SHA256CryptoServiceProvider())
            {
                var blockHash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(data)));

                return blockHash == blockToValidate.BlockHash;
            }
        }
    }
}
