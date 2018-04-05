using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Messages.Validation;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;

namespace WebAppP2P.Core.Tests.Messages
{
    public class MessageHashCashValidatorTests
    {
        [Fact]
        public void MessageHashCashValidator_Validate_Should_Return_False_When_Nonce_Is_Incorrect()
        {
            var valdiator = new MessageHashCashValidator(new HashCash());
            var msg = MessageHelper.GetTestEncryptedMessage();

            var result = valdiator.Validate(msg);

            Assert.True(result == true);
        }

        [Fact]
        public void MessageHashCashValidator_Validate_Should_Return_True_When_Nonce_Is_Correct()
        {
            var valdiator = new MessageHashCashValidator(new HashCash());
            var msg = MessageHelper.GetTestEncryptedMessage();

            msg.Nonce--;
            var result = valdiator.Validate(msg);

            Assert.True(result == false);
        }
    }
}
