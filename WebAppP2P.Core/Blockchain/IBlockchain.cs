using System.Threading.Tasks;

namespace WebAppP2P.Core.Blockchain
{
    public interface IBlockchain
    {
        Task<BlockchainAddResult> AddAsync(Block newBlock);
        Task<Block> ComputeNextAsync();
        Task<Block> GetHeadBlockAsync();
        Task<Block> GetNext(string blockHash);
    }
}