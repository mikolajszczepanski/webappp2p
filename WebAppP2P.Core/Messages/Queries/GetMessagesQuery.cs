using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Messages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebAppP2P.Core.Database.Queries;
using WebAppP2P.Core.Database;

namespace WebAppP2P.Core.Messages.Queries
{
    public enum EncryptedMessageType
    {
        All,
        OnlyInBlockchain,
        OnlyOuterBlockchain
    }

    public class GetMessagesQuery : IQuery<IEnumerable<EncryptedMessageStore>>
    {
        public EncryptedMessageType Type { get; set; }
    }

    public class GetMessagesQueryHandler : IQueryHandler<GetMessagesQuery, IEnumerable<EncryptedMessageStore>>
    {
        private readonly ApplicationDatabase _applicationDatabase;

        public GetMessagesQueryHandler(ApplicationDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        public IEnumerable<EncryptedMessageStore> Handle(GetMessagesQuery query)
        {
            return _applicationDatabase.Messages
                .Include(m => m.BlockMessages)
                .Where(m =>
                    (query.Type == EncryptedMessageType.All) ||
                    (query.Type == EncryptedMessageType.OnlyInBlockchain && m.BlockMessages.Count > 0) ||
                    (query.Type == EncryptedMessageType.OnlyOuterBlockchain && m.BlockMessages.Count == 0)
                )
                .ToList();
        }
    }
}
