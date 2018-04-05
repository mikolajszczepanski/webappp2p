using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAppP2P.Core.Messages
{
    public interface IMessageStore
    {
        Task<bool> TryAddAsync(EncryptedMessage message);
        bool TryGet(string id, out EncryptedMessage message);
        IEnumerable<EncryptedMessage> GetAll();
        IEnumerable<EncryptedMessage> Get(Func<EncryptedMessage,bool> predicate);
    }
}