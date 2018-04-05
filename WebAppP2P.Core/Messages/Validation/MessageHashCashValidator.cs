using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppP2P.Core.Messages.Validation
{
    internal class MessageHashCashValidator : IMessageValidator
    {
        private readonly IHashCash _hashCash;

        public MessageHashCashValidator(IHashCash hashCash)
        {
            _hashCash = hashCash;
        }

        public bool Validate(EncryptedMessage message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(message.IV);
            sb.Append(message.ToKey);
            sb.Append(message.Timestamp);
            sb.Append(message.Title);
            sb.Append(message.Content);
            sb.Append(message.To);
            sb.Append(message.From);
            return _hashCash.Validate(sb.ToString(), message.Nonce);
        }
    }
}
