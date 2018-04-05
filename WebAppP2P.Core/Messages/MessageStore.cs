using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Messages
{
    public class MessageStore : IMessageStore
    {
        private readonly IMessageValidator _messageValidator;
        private readonly ApplicationDatabase _applicationDatabase;

        public MessageStore(IMessageValidator messageValidator, ApplicationDatabase applicationDatabase)
        {
            _messageValidator = messageValidator;
            _applicationDatabase = applicationDatabase;
        }

        public IEnumerable<EncryptedMessage> Get(Func<EncryptedMessage, bool> predicate)
        {
            return _applicationDatabase.Messages.Select(Convert).Where(predicate).ToList();
        }

        public IEnumerable<EncryptedMessage> GetAll()
        {
            return _applicationDatabase.Messages.Select(Convert).ToList();
        }

        public async Task<bool> TryAddAsync(EncryptedMessage message)
        {
            var id = message.Id;

            if (!_messageValidator.Validate(message))
            {
                return false;
            }
            if (GetMessageById(message.Id) != null)
            {
                return false;
            }

            try
            {
                var messageDb = new EncryptedMessageStore(message);
                _applicationDatabase.Messages.Add(messageDb);
                return await _applicationDatabase.SaveChangesAsync() > 0;
            }
            catch(DbException)
            {
                return false;
            }
        }

        public bool TryGet(string id, out EncryptedMessage message)
        {
            message = GetMessageById(id);
            return message != null ? true : false;
        }

        private EncryptedMessage GetMessageById(string id)
        {
            return _applicationDatabase.Messages.Select(Convert).Where(m => m.Id == id).SingleOrDefault();
        }

        private Func<EncryptedMessageStore, EncryptedMessage> Convert = (message) =>
        {
            return new EncryptedMessage()
            {
                Content = message.Content,
                From = message.From,
                FromKey = message.FromKey,
                Id = message.Id,
                IV = message.IV,
                Nonce = message.Nonce,
                Timestamp = message.Timestamp,
                Title = message.Title,
                To = message.To,
                ToKey = message.ToKey
            };
        };
    }
}
