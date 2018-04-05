using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Messages.Validation;
using WebAppP2P.Core.Tests.Helpers;
using Xunit;

namespace WebAppP2P.Core.Tests.Messages
{
    public class MessageConsensusValidatorTests
    {
        [Fact]
        public void MessageConsensusValidator_Validate_Should_Return_False_When_Content_Is_Too_Long()
        {
            var validator = new MessageConsensusValidator();
            var sb = new StringBuilder();
            for (int i = 0; i < ( MessagesConsensus.MAX_CONTENT_BYTES / 4 ); i++)
            {
                sb.Append("test");
            }
            var msg = MessageHelper.GetTestEncryptedMessage(content: sb.ToString());

            var result = validator.Validate(msg);

            Assert.True(result == false);
        }

        [Fact]
        public void MessageConsensusValidator_Validate_Should_Return_False_When_Title_Is_Too_Long()
        {
            var validator = new MessageConsensusValidator();
            var sb = new StringBuilder();
            for (int i = 0; i < ( MessagesConsensus.MAX_TITLE_BYTES / 4 ); i++)
            {
                sb.Append("test");
            }
            var msg = MessageHelper.GetTestEncryptedMessage(title: sb.ToString());

            var result = validator.Validate(msg);

            Assert.True(result == false);
        }

        [Fact]
        public void MessageConsensusValidator_Validate_Should_Return_False_When_Timestamp_Is_After_Now()
        {
            var validator = new MessageConsensusValidator();
            var msg = MessageHelper.GetTestEncryptedMessage();
            msg.Timestamp = DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds();

            var result = validator.Validate(msg);

            Assert.True(result == false);
        }

        [Fact]
        public void MessageConsensusValidator_Validate_Should_Return_True_When_Data_Is_Correct()
        {
            var validator = new MessageConsensusValidator();
            var msg = MessageHelper.GetTestEncryptedMessage();

            var result = validator.Validate(msg);

            Assert.True(result == true);
        }
    }
}
