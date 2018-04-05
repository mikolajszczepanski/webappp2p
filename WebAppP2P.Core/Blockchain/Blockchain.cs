using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Blockchain.Queries;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Database.Queries;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Messages.Queries;

namespace WebAppP2P.Core.Blockchain
{
    public enum BlockchainAddResultError
    {
        None,
        LackOfPreviousBlock,
        ValidationError,
        InternalError,
        AlreadyAdded
    }

    public class BlockchainAddResult
    {
        public bool IsSuccessful { get; set; }
        public BlockchainAddResultError Error { get; set; }
    }

    public class Blockchain : IBlockchain
    {
        private readonly IBlockchainCreator _blockchainCreator;
        private readonly IBlockchainValidator _blockchainValidator;
        private readonly IMessageStore _messageStore;
        private readonly IQueryHandler<GetBlockQuery, Database.Block> _queryHandlerGetBlock;
        private readonly IQueryHandler<GetHeadBlockQuery, Database.Block> _queryHandlerGetHeadBlock;
        private readonly IQueryHandler<GetMessagesQuery, IEnumerable<EncryptedMessageStore>> _queryHandlerGetMessages;
        private readonly IQueryHandler<AddBlockQuery, bool> _queryHandlerAddBlock;
        private readonly IQueryHandler<MarkMainChainQuery, bool> _queryHandlerMarkMainChain;
        private readonly BlockchainHashCash _hashCash;

        public Blockchain(
            IBlockchainCreator blockchainCreator,
            IBlockchainValidator blockchainValidator,
            IMessageStore messageStore,
            IQueryHandler<GetBlockQuery, Database.Block> queryHandlerGetBlock,
            IQueryHandler<GetHeadBlockQuery, Database.Block> queryHandlerGetHeadBlock,
            IQueryHandler<GetMessagesQuery, IEnumerable<EncryptedMessageStore>> queryHandlerGetMessages,
            IQueryHandler<AddBlockQuery, bool> queryHandlerAddBlock,
            IQueryHandler<MarkMainChainQuery, bool> queryHandlerMarkMainChain
            )
        {
            _blockchainCreator = blockchainCreator;
            _blockchainValidator = blockchainValidator;
            _messageStore = messageStore;
            _queryHandlerGetBlock = queryHandlerGetBlock;
            _queryHandlerGetHeadBlock = queryHandlerGetHeadBlock;
            _queryHandlerGetMessages = queryHandlerGetMessages;
            _queryHandlerAddBlock = queryHandlerAddBlock;
            _queryHandlerMarkMainChain = queryHandlerMarkMainChain;
            _hashCash = new BlockchainHashCash(3);
        }

        public async Task<Block> GetHeadBlockAsync()
        {
            var dbHeadBlock = _queryHandlerGetHeadBlock.Handle(new GetHeadBlockQuery());
            return new Block(dbHeadBlock);
        }

        public async Task<Block> GetNext(string blockHash)
        {
            var nextBlock = _queryHandlerGetBlock.Handle(new GetBlockQuery()
            {
                BlockHash = blockHash,
                Type = GetBlockQueryType.PreviousInMainChain
            });
            if(nextBlock == null)
            {
                return null;
            }
            return new Block(nextBlock);
        }

        public async Task<Block> ComputeNextAsync()
        {
            var headBlock = _queryHandlerGetHeadBlock.Handle(new GetHeadBlockQuery());

            var messages = _queryHandlerGetMessages.Handle(new GetMessagesQuery()
            {
                Type = EncryptedMessageType.OnlyOuterBlockchain
            });

            var block = new Block(headBlock);

            var newBlock = _blockchainCreator.Create(block, messages, _hashCash.GetNonce);

            return newBlock;
        }

        public async Task<BlockchainAddResult> AddAsync(Block newBlock)
        {
            var duplicateBlock = _queryHandlerGetBlock.Handle(new GetBlockQuery()
            {
                BlockHash = newBlock.BlockHash
            });
            if(duplicateBlock != null)
            {
                return new BlockchainAddResult()
                {
                    IsSuccessful = false,
                    Error = BlockchainAddResultError.AlreadyAdded
                };
            }

            var previousBlockDb = _queryHandlerGetBlock.Handle(new GetBlockQuery()
            {
                BlockHash = newBlock.BlockHashPrevious
            });

            if(previousBlockDb == null)
            {
                return new BlockchainAddResult()
                {
                    IsSuccessful = false,
                    Error = BlockchainAddResultError.LackOfPreviousBlock
                };
            }

            var validationResult = _blockchainValidator.Validate(newBlock, new Block(previousBlockDb), _hashCash.Validate);

            if (!validationResult)
            {
                return new BlockchainAddResult()
                {
                    IsSuccessful = false,
                    Error = BlockchainAddResultError.ValidationError
                };
            }

            foreach (var msg in newBlock.Messages)
            {
                var result = await _messageStore.TryAddAsync(msg);
            }

            var queryResult = _queryHandlerAddBlock.Handle(new AddBlockQuery()
            {
                NewBlock = newBlock
            });

            if (!queryResult)
            {
                return new BlockchainAddResult()
                {
                    IsSuccessful = false,
                    Error = BlockchainAddResultError.InternalError
                };
            }

            var markResult = _queryHandlerMarkMainChain.Handle(new MarkMainChainQuery() { });

            if (!markResult)
            {
                return new BlockchainAddResult()
                {
                    IsSuccessful = false,
                    Error = BlockchainAddResultError.InternalError
                };
            }

            return new BlockchainAddResult()
            {
                IsSuccessful = true,
                Error = BlockchainAddResultError.None
            };
        }



    }
}
