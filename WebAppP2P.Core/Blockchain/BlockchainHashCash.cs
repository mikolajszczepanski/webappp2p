using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;

namespace WebAppP2P.Core.Blockchain
{
    public class BlockchainHashCash : HashCash
    {
        private int _zeroBytes;

        public BlockchainHashCash(int zeroBytes)
        {
            _zeroBytes = zeroBytes;
        }

        protected override int ZeroBytes => _zeroBytes;
    }
}
