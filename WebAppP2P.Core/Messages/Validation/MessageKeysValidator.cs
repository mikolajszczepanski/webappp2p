using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WebAppP2P.Core.Messages.Validation
{
    internal class MessageKeysValidator : IMessageValidator
    {
        public bool Validate(EncryptedMessage message)
        {
            return Validate(message.To) && Validate(message.From);
        }

        private bool Validate(string publicKey)
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportCspBlob(Convert.FromBase64String(publicKey));
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
