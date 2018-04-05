using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace WebAppP2P.Core.Messages
{
    public class HashCash : IHashCash
    {
        private const int ZERO_BYTES_DEFAULT = 2;

        protected virtual int ZeroBytes { get
            {
                return ZERO_BYTES_DEFAULT;
            }
        }

        public ulong GetNonce(string data)
        {
            var random = new Random();
            ulong startNonce = (ulong)random.Next(int.MaxValue);
            ulong nonce = startNonce;
            while (!Validate(data, nonce))
            {
                nonce++;
            }

            return nonce;
        }

        public bool Validate(string data, ulong nonce)
        {
            using(var sha = SHA256.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data + nonce.ToString());
                var hash = sha.ComputeHash(buffer);
                for (int i = 0; i < ZeroBytes; i++)
                {
                    if(hash[i] != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

    }
}
