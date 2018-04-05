using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppP2P.Core.Blockchain;
using WebAppP2P.Dto.Api;

namespace WebAppP2P.Api
{
    public class BlockchainApi : Controller
    {
        private readonly IBlockchain _blockchain;

        public BlockchainApi(IBlockchain blockchain)
        {
            _blockchain = blockchain;
        }

        [HttpGet("api/blockchain/latest")]
        public async Task<ActionResult> GetLatest()
        {
            var block = await _blockchain.GetHeadBlockAsync();
            return Ok(block);
        }

        [HttpGet("api/blockchain/next/{id}")]
        public async Task<ActionResult> GetNext(string id)
        {
            var blockHash = UrlUtils.UrlDecodeWithBase64(id);
            var block = await _blockchain.GetNext(blockHash);
            if(block == null)
            {
                return NotFound();
            }
            return Ok(block);
        }

        [HttpPost("api/blockchain")]
        public async Task<ActionResult> PostBlock([FromBody] Block block)
        {
            var result = await _blockchain.AddAsync(block);
            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
