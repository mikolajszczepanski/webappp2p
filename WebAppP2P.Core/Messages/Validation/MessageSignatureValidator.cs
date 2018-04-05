using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using WebAppP2P.Core.Keys;

namespace WebAppP2P.Core.Messages.Validation
{
    internal class MessageSignatureValidator : IMessageValidator
    {
        public bool Validate(EncryptedMessage message)
        {
            var senderPublicKey = Convert.FromBase64String(message.From);
            using (var rsa = new RSACryptoServiceProvider(KeysConfiguration.RSAKeysBits))
            {
                rsa.ImportCspBlob(senderPublicKey);
                rsa.PersistKeyInCsp = false;

                var dataToVerify = new StringBuilder();
                dataToVerify.Append(message.IV);
                dataToVerify.Append(message.ToKey);
                dataToVerify.Append(message.Timestamp);
                dataToVerify.Append(message.Title);
                dataToVerify.Append(message.Content);
                dataToVerify.Append(message.To);
                dataToVerify.Append(message.From);
                var dataToVerifyBytes = Encoding.UTF8.GetBytes(dataToVerify.ToString());

                var isCorrect = rsa.VerifyData(dataToVerifyBytes,
                    new SHA256CryptoServiceProvider(),
                    Convert.FromBase64String(message.Id)
                    );
                return isCorrect;
            }
        }
    }
}
