using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using WebAppP2P.Core.Keys;
using WebAppP2P.Core.Messages.Validation;

namespace WebAppP2P.Core.Messages
{
    public class MessageValidator : IMessageValidator
    {
        private readonly IList<IMessageValidator> _validators = new List<IMessageValidator>();

        public MessageValidator()
        {
            _validators.Add(new MessageConsensusValidator());
            _validators.Add(new MessageKeysValidator());
            _validators.Add(new MessageHashCashValidator(new HashCash()));
            _validators.Add(new MessageSignatureValidator());
        }

        public bool Validate(EncryptedMessage message)
        {
            foreach (var validator in _validators)
            {
                if (!validator.Validate(message))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
