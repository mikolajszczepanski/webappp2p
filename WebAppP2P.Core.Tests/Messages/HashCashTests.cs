using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;
using Xunit;

namespace WepAppP2P.Core.Tests.Messages
{
    public class HashCashTests
    {
        [Fact]
        public void HashCash_GetNonce_Should_Return_Valid_Nonce()
        {
            var hc = new HashCash();
            var data = "test";

            var nonce = hc.GetNonce(data);

            Assert.True(hc.Validate(data, nonce));
        }

        [Fact]
        public void HashCash_Validate_Should_Return_False_When_Nonce_Is_Invalid()
        {
            var hc = new HashCash();
            var data = "test";

            var nonce = hc.GetNonce(data) - 1;

            Assert.False(hc.Validate(data, nonce));
        }
    }
}
