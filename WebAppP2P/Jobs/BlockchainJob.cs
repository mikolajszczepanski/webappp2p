using FluentScheduler;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebAppP2P.Api;
using WebAppP2P.Core.Blockchain;
using WebAppP2P.Core.Nodes;
using WebAppP2P.Services;

namespace WebAppP2P.Jobs
{

    public class BlockchainJob : IJob
    {
        private readonly IBlockchain _blockchain;
        private readonly INodesRepository _nodesRepository;
        private readonly IPeerCommunicationService _peerCommunicationService;
        private readonly IOptionsSnapshot<NodeOptions> _nodeOptions;

        public BlockchainJob(IBlockchain blockchain,
                             INodesRepository nodesRepository,
                             IPeerCommunicationService peerCommunicationService,
                             IOptionsSnapshot<NodeOptions> nodeOptions)
        {
            _blockchain = blockchain;
            _nodesRepository = nodesRepository;
            _peerCommunicationService = peerCommunicationService;
            _nodeOptions = nodeOptions;
        }

        public async void Execute()
        {
            try
            {
                Console.WriteLine("\tBlockchain {0}", DateTimeOffset.Now);
                Block headBlock = await _blockchain.GetHeadBlockAsync();
                Block tempBlock = headBlock;
                do
                {
                    var blocks = GetNextBlockFromNodes(tempBlock.BlockHash);
                    tempBlock = null;
                    foreach (var nextBlock in blocks)
                    {
                        if (nextBlock != null)
                        {
                            Console.WriteLine("\tLoading new block: {0}", nextBlock.BlockHash);
                            var result = await _blockchain.AddAsync(nextBlock);
                            if (result.IsSuccessful)
                            {
                                Console.WriteLine("\tLoaded new block: {0}", nextBlock.BlockHash);
                                tempBlock = nextBlock;
                                break;
                            }
                        }
                    }
                } while (tempBlock != null);
                if (tempBlock == null)
                {
                    Console.WriteLine("\tCreating new block...");
                    var createdBlock = await _blockchain.ComputeNextAsync();
                    var result = await _blockchain.AddAsync(createdBlock);
                    if (result.IsSuccessful)
                    {
                        Console.WriteLine("\tCreated new block: {0}", createdBlock.BlockHash);
                        await SendNewBlockAsync(createdBlock);
                    }
                    else
                    {
                        Console.WriteLine("\tFail to create new block");
                    }
                }
                //TODO delete branches old than 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //TODO log error
            }
        }

        private async Task SendNewBlockAsync(Block createdBlock)
        {
            var nodes = _nodesRepository.GetNodes(NodesRepositoryFilter.OnlyActive);
            await _peerCommunicationService.SendAsync(nodes, Endpoints.Blockchain, createdBlock);
        }

        private IEnumerable<Block> GetNextBlockFromNodes(string headBlockHash)
        {
            foreach (var node in _nodesRepository.GetNodes(NodesRepositoryFilter.OnlyActive))
            {
                var headBlockHashEncoded = WebUtility.UrlEncode(headBlockHash);
                var block = _peerCommunicationService.GetAsync<Block>(node, Endpoints.Blockchain + "/next/" + headBlockHashEncoded);
                if(block != null && block.Result != null)
                {
                    yield return block.Result;
                }
            }
        }
    }
}
