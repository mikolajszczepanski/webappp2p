using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WebAppP2P.Core.Keys
{
    public class KeysFactory : IKeysFactory
    {
        public KeysPair GenerateNewPair()
        {
            using (var rsa = new RSACryptoServiceProvider(KeysConfiguration.RSAKeysBits))
            {
                rsa.PersistKeyInCsp = false;
                return new KeysPair()
                {
                    PrivateKey = rsa.ExportCspBlob(true),
                    PublicKey = rsa.ExportCspBlob(false)
                };
            }
        }
    }
}
