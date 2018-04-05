using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Messages;

namespace WebAppP2P.Core.Blockchain
{
    public class BlockchainCreator : IBlockchainCreator
    {
        public Block Create(Block headBlock, IEnumerable<EncryptedMessage> messages, Func<string,ulong> getNonce)
        {
            var sortedMessages = messages.ToList();
            sortedMessages.Sort((m1, m2) => m1.Id.CompareTo(m2.Id));
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var newLength = headBlock.Length + 1;

            StringBuilder sb = new StringBuilder();
            foreach (var msg in sortedMessages)
            {
                sb.Append(msg.Id);
            }
            sb.Append(newLength);
            sb.Append(timestamp);
            sb.Append(headBlock.BlockHash);
            var data = sb.ToString();

            using (var sha = new SHA256CryptoServiceProvider())
            {

                var newBlock = new Block()
                {
                    BlockHashPrevious = headBlock.BlockHash,
                    Length = newLength,
                    Timestamp = timestamp,
                    Nonce = getNonce(data),
                    BlockHash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(data))),
                    Messages = messages.Select(m => new EncryptedMessage()
                    {
                        Content = m.Content,
                        From = m.From,
                        FromKey = m.FromKey,
                        Id = m.Id,
                        IV = m.IV,
                        Nonce = m.Nonce,
                        Timestamp = m.Timestamp,
                        Title = m.Title,
                        To = m.Title,
                        ToKey = m.ToKey
                    }).ToList()
                };

                return newBlock;
            }
        }
    }
}
