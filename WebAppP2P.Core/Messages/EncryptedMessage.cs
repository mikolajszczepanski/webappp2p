using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Messages
{
    public class EncryptedMessage
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long Timestamp { get; set; }
        public string ToKey { get; set; }
        public string FromKey { get; set; }
        public string IV { get; set; }
        public ulong Nonce { get; set; }

        public EncryptedMessage()
        {

        }

        public EncryptedMessage(EncryptedMessageStore messageStore)
        {
            Id = messageStore.Id;
            From = messageStore.From;
            To = messageStore.To;
            Title = messageStore.Title;
            Content = messageStore.Content;
            Timestamp = messageStore.Timestamp;
            ToKey = messageStore.ToKey;
            FromKey = messageStore.FromKey;
            IV = messageStore.IV;
            Nonce = messageStore.Nonce;
        }
    }
}
