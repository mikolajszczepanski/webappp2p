using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Messages.Validation;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;

namespace WebAppP2P.Core.Tests.Messages
{
    public class MessageKeysValidatorTests
    {
        [Fact]
        public void MessageKeysValidator_Validate_Should_Return_True_When_Public_Keys_Are_Valid()
        {
            var valdiator = new MessageKeysValidator();
            var msg = MessageHelper.GetTestEncryptedMessage();

            var result = valdiator.Validate(msg);

            Assert.True(result == true);
        }

        [Fact]
        public void MessageKeysValidator_Validae_Should_Return_False_When_To_Public_Key_Is_Invalid()
        {
            var valdiator = new MessageKeysValidator();
            var msg = MessageHelper.GetTestEncryptedMessage();
            msg.To = MessageHelper.GetInvalidData();

            var result = valdiator.Validate(msg);

            Assert.True(result == false);
        }

        [Fact]
        public void MessageKeysValidator_Validae_Should_Return_False_When_From_Public_Key_Is_Invalid()
        {
            var valdiator = new MessageKeysValidator();
            var msg = MessageHelper.GetTestEncryptedMessage();
            msg.From = MessageHelper.GetInvalidData();

            var result = valdiator.Validate(msg);

            Assert.True(result == false);
        }
       
    }
}
