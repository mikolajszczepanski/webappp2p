using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using WebAppP2P.Core.Keys;

namespace WebAppP2P.Core.Messages
{
    public class EncryptedMessageBuilder : IEncryptedMessageBuilder
    {
        private readonly EncryptedMessage _message;
        private string _content;
        private string _title;
        private readonly IHashCash _hashCash;

        public EncryptedMessageBuilder(IHashCash hashCash)
        {
            _message = new EncryptedMessage();
            _hashCash = hashCash;
        }

        public IEncryptedMessageBuilder AddContent(string content)
        {
            _content = content;
            return this;
        }

        public IEncryptedMessageBuilder AddTitle(string title)
        {
            _title = title;
            return this;
        }

        public IEncryptedMessageBuilder AddReciever(string recieverPublicKeyBase64)
        {
            _message.To = recieverPublicKeyBase64;
            return this;
        }

        public IEncryptedMessageBuilder AddSender(string senderPublicKeyBase64)
        {
            _message.From = senderPublicKeyBase64;
            return this;
        }

        public EncryptedMessage EncryptAndBuild(string senderPrivateKeyBase64)
        {
            _message.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                aes.GenerateKey();
                aes.GenerateIV();
                var aesEncryptor = aes.CreateEncryptor();

                _message.IV = Convert.ToBase64String(aes.IV);
                _message.Title = EncryptData(aesEncryptor, _title);
                _message.Content = EncryptData(aesEncryptor, _content);

                //Encrypt AES key for reciever
                using (var rsa = new RSACryptoServiceProvider(KeysConfiguration.RSAKeysBits))
                {
                    rsa.ImportCspBlob(Convert.FromBase64String(_message.To));
                    rsa.PersistKeyInCsp = false;

                    _message.ToKey = Convert.ToBase64String(rsa.Encrypt(aes.Key, true));
                }

                //Encrypt AES key for sender
                using (var rsa = new RSACryptoServiceProvider(KeysConfiguration.RSAKeysBits))
                {
                    rsa.ImportCspBlob(Convert.FromBase64String(_message.From));
                    rsa.PersistKeyInCsp = false;

                    _message.FromKey = Convert.ToBase64String(rsa.Encrypt(aes.Key, true));
                }

                //Create Id/Signature
                using (var rsa = new RSACryptoServiceProvider(KeysConfiguration.RSAKeysBits))
                {
                    rsa.ImportCspBlob(Convert.FromBase64String(senderPrivateKeyBase64));
                    rsa.PersistKeyInCsp = false;

                    var dataToSign = new StringBuilder();
                    dataToSign.Append(_message.IV);
                    dataToSign.Append(_message.ToKey);
                    dataToSign.Append(_message.Timestamp);
                    dataToSign.Append(_message.Title);
                    dataToSign.Append(_message.Content);
                    dataToSign.Append(_message.To);
                    dataToSign.Append(_message.From);
                    var dataToSignBytes = Encoding.UTF8.GetBytes(dataToSign.ToString());
                    _message.Id = Convert.ToBase64String(
                        rsa.SignData(dataToSignBytes, new SHA256CryptoServiceProvider())
                        );
                    _message.Nonce = _hashCash.GetNonce(dataToSign.ToString());
                }
            }
            return _message;
        }

        private string EncryptData(ICryptoTransform encryptor, string data)
        {
            using (var memoryStream = new MemoryStream())
            {
                var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                var dataToEncrypt = Encoding.UTF8.GetBytes(data);
                cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }
}
