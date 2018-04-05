using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Keys;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Messages.Validation;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;

namespace WepAppP2P.Core.Tests.Messages
{
    public class MessageSignatureValidatorTests
    {

        [Fact]
        public void MessageValidator_ValidSignature_Should_Return_True_When_Has_Valid_Signature()
        {
            EncryptedMessage msg = MessageHelper.GetTestEncryptedMessage();

            var md = new MessageSignatureValidator();
            var result = md.Validate(msg);

            Assert.True(result == true);
        }

        [Fact]
        public void MessageValidator_ValidSignature_Should_Return_False_When_Title_Is_Invalid()
        {
            EncryptedMessage msg = MessageHelper.GetTestEncryptedMessage();
            msg.Title = MessageHelper.GetInvalidData();

            var md = new MessageSignatureValidator();
            var result = md.Validate(msg);

            Assert.True(result == false);
        }

        [Fact]
        public void MessageValidator_ValidSignature_Should_Return_False_When_Content_Is_Invalid()
        {
            EncryptedMessage msg = MessageHelper.GetTestEncryptedMessage();
            msg.Content = MessageHelper.GetInvalidData();

            var md = new MessageSignatureValidator();
            var result = md.Validate(msg);

            Assert.True(result == false);
        }

    }
}
