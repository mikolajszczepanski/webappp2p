using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppP2P.Core.Messages.Validation
{
    internal class MessageConsensusValidator : IMessageValidator
    {
        public bool Validate(EncryptedMessage message)
        {
            var titleBytes = Encoding.UTF8.GetByteCount(message.Title);
            var contentBytes = Encoding.UTF8.GetByteCount(message.Content);

            if(titleBytes > MessagesConsensus.MAX_TITLE_BYTES)
            {
                return false;
            }
            if(contentBytes > MessagesConsensus.MAX_CONTENT_BYTES)
            {
                return false;
            }
            if(message.Timestamp > DateTimeOffset.Now.ToUnixTimeSeconds())
            {
                return false;
            }

            return true;
        }
    }
}
