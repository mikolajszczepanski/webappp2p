using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppP2P.Core.Messages
{
    public class DecryptedMessageDto
    {

        public string From { get; set; }
        public string To { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Id { get; set; }
        public long Timestamp { get; set; }
    }
}
