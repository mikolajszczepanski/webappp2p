using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppP2P.Tests.Helpers
{
    public class MockMessageStore : IMessageStore
    {
        public IDictionary<string,EncryptedMessage> State = new Dictionary<string,EncryptedMessage>();

        public IEnumerable<EncryptedMessage> Get(Func<EncryptedMessage, bool> predicate)
        {
            return State.Where(x => predicate(x.Value)).Select(x => x.Value);
        }

        public IEnumerable<EncryptedMessage> GetAll()
        {
            return State.Select(m => m.Value);
        }

        public Task<bool> TryAddAsync(EncryptedMessage message)
        {
            return Task.FromResult(State.TryAdd(message.Id, message));
        }

        public bool TryGet(string id, out EncryptedMessage message)
        {
            return State.TryGetValue(id, out message);
        }
    }
}
