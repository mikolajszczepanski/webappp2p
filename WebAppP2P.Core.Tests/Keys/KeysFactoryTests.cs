using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Keys;
using Xunit;

namespace WepAppP2P.Core.Tests.Keys
{
    public class KeysFactoryTests
    {
        [Fact]
        public void KeysFactory_GenerateNewPair_Should_Generate_NotNull_KeysPair()
        {
            var kf = new KeysFactory();

            var keys = kf.GenerateNewPair();

            Assert.NotNull(keys);
            Assert.NotNull(keys.PrivateKey);
            Assert.NotNull(keys.PublicKey);
            Assert.True(keys.PrivateKey.Length > 0);
            Assert.True(keys.PublicKey.Length > 0);
        }
    }
}
