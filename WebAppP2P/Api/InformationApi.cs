using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Nodes;
using WebAppP2P.Dto;
using WebAppP2P.Dto.Api;

namespace WebAppP2P.Api
{
    public class InformationApi : Controller
    {
        private readonly INodesRepository _nodesRepository;

        public InformationApi(INodesRepository nodesRepository)
        {
            _nodesRepository = nodesRepository;
        }

        [HttpGet("api/information/status")]
        public ActionResult GetStatus()
        {
            return Ok(new InformationStatus()
            {
                Ready = true
            });
        }

        [HttpGet("api/information/nodes")]
        public ActionResult GetNodes()
        {
            return Ok(new InformationNodes()
            {
                Nodes = _nodesRepository.GetNodes(NodesRepositoryFilter.All),
            });
        }

        [HttpPost("api/information/nodes")]
        public ActionResult PostNodes([FromBody] InformationNodesAdd nodes)
        {
            Console.WriteLine("Got new nodes({0}) on api/information/nodes", nodes.Nodes.Count());
            _nodesRepository.AddNodesAsync(nodes.Nodes);
            return Ok();
        }

    }
}
