using System;
using System.Security.Cryptography;
using System.Linq;

namespace WebAppP2P.Core.Keys
{
    public class KeysValidator : IKeysValidator
    {
        public bool VerifyKeys(KeysPair keys)
        {
            if (keys == null || 
                keys.PublicKey == null || 
                keys.PrivateKey == null || 
                keys.PublicKey.Length <= 0 || 
                keys.PrivateKey.Length <= 0)
            {
                return false;
            }
            try
            {
                var rsaFromPrivateKey = new RSACryptoServiceProvider();
                rsaFromPrivateKey.ImportCspBlob(keys.PrivateKey);
                var rsaPublicKeyFromPrivateKey = rsaFromPrivateKey.ExportCspBlob(false);

                if (!rsaPublicKeyFromPrivateKey.SequenceEqual(keys.PublicKey))
                {
                    return false;
                }

                var rsaFromPublicKey = new RSACryptoServiceProvider();
                rsaFromPublicKey.ImportCspBlob(keys.PublicKey);

                var rsaFromPublicKeyParameters = rsaFromPublicKey.ExportParameters(false);
                var rsaFromPrivateKeyParameters = rsaFromPrivateKey.ExportParameters(true);
                
                if(!rsaFromPublicKeyParameters.Exponent.SequenceEqual(rsaFromPrivateKeyParameters.Exponent) ||
                    !rsaFromPublicKeyParameters.Modulus.SequenceEqual(rsaFromPrivateKeyParameters.Modulus))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
