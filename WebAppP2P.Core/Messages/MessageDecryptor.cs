using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using WebAppP2P.Core.Keys;

namespace WebAppP2P.Core.Messages
{
    public class MessageDecryptor : IMessageDecryptor
    {
        /// <exception cref="Exception"></exception>
        /// <param name="message"></param>
        /// <param name="privateKey">Must be base64 string</param>
        /// <returns></returns>
        public DecryptedMessageDto Decrypt(EncryptedMessage message, string privateKey, bool privateKeyIsReciever)
        {
            var recieverPrivateKey = Convert.FromBase64String(privateKey);
            using (var rsa = new RSACryptoServiceProvider(KeysConfiguration.RSAKeysBits))
            {
                rsa.ImportCspBlob(recieverPrivateKey);
                rsa.PersistKeyInCsp = false;

                var aesKey = rsa.Decrypt(Convert.FromBase64String(privateKeyIsReciever ? message.ToKey : message.FromKey), true);

                using(var aes = new AesCryptoServiceProvider())
                {
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;

                    aes.Key = aesKey;
                    aes.IV = Convert.FromBase64String(message.IV);
                    var aesDecryptor = aes.CreateDecryptor();

                    DecryptedMessageDto messageDto = new DecryptedMessageDto();
                    messageDto.Id = message.Id;
                    messageDto.To = message.To;
                    messageDto.From = message.From;
                    messageDto.Timestamp = message.Timestamp;
                    messageDto.Title = DecryptData(aesDecryptor, message.Title);
                    messageDto.Content = DecryptData(aesDecryptor, message.Content);
                    return messageDto;
                } 
            }
        }

        private string DecryptData(ICryptoTransform encryptor, string data)
        {
            using (var memoryStream = new MemoryStream())
            {
                var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                var dataToEncrypt = Convert.FromBase64String(data);
                cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                cryptoStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
