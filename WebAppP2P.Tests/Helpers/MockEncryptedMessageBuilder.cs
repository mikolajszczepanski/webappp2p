using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Messages;

namespace WebAppP2P.Tests.Helpers
{
    public class MockEncryptedMessageBuilder : IEncryptedMessageBuilder
    {
        private static int CurrentId = -1;
        private string _to;
        private string _from;
        private string _content;
        private string _title;


        public IEncryptedMessageBuilder AddContent(string content)
        {
            _content = content;
            return this;
        }

        public IEncryptedMessageBuilder AddReciever(string recieverPublicKey)
        {
            _to = recieverPublicKey;
            return this;
        }

        public IEncryptedMessageBuilder AddSender(string senderPublicKey)
        {
            _from = senderPublicKey;
            return this;
        }

        public IEncryptedMessageBuilder AddTitle(string title)
        {
            _title = title;
            return this;
        }

        public EncryptedMessage EncryptAndBuild(string senderPrivateKey)
        {
            CurrentId++;
            return new EncryptedMessage()
            {
                Content = "ENCRYPTED_CONTENT_" + _content,
                Id = "ID_MOCK_" + CurrentId,
                From = _from,
                To = _to,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Title = "ENCRYPTED_TITLE_" + _title
            };
        }
    }
}
