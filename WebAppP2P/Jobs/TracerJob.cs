using FluentScheduler;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppP2P.Api;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Database.Queries;
using WebAppP2P.Core.Nodes;
using WebAppP2P.Core.Nodes.Queries;
using WebAppP2P.Dto;
using WebAppP2P.Dto.Api;
using WebAppP2P.Services;

namespace WebAppP2P.Jobs
{
    public class TracerJob : IJob
    {
        private readonly INodesRepository _nodesRepository;
        private readonly IPeerCommunicationService _peerCommunicationService;
        private readonly IQueryHandler<CalculateNodeStatisticsQuery> _queryHandlerCalculateStatistics;
        private readonly IQueryHandler<ClearNodeStatisticsQuery> _queryHandlerClearStatistics;
        private readonly IOptionsSnapshot<NodeOptions> _nodeOptions;

        private object _lock = new object();

        public TracerJob(INodesRepository nodesRepository,
                      IPeerCommunicationService peerCommunicationService,
                      IQueryHandler<CalculateNodeStatisticsQuery> queryHandlerCalculateStatistics,
                      IQueryHandler<ClearNodeStatisticsQuery> queryHandlerClearStatistics,
                      IOptionsSnapshot<NodeOptions> nodeOptions
            )
        {
            _nodesRepository = nodesRepository;
            _peerCommunicationService = peerCommunicationService;
            _queryHandlerCalculateStatistics = queryHandlerCalculateStatistics;
            _queryHandlerClearStatistics = queryHandlerClearStatistics;
            _nodeOptions = nodeOptions;
        }

        public async void Execute()
        {
            try
            {
                Console.WriteLine("\tTracer {0}", DateTimeOffset.Now);
                await NetworkDiscovery();
                _queryHandlerCalculateStatistics.Handle(new CalculateNodeStatisticsQuery()
                {
                    MinTimestamp = DateTimeOffset.Now.AddHours(-1 * _nodeOptions.Value.Tracer.StatisticsConsider).ToUnixTimeSeconds(),
                    MaxActives = _nodeOptions.Value.Tracer.MaxNodesActive
                });
                _queryHandlerClearStatistics.Handle(new ClearNodeStatisticsQuery()
                {
                    MinTimestamp = DateTimeOffset.Now.AddSeconds(-1 * _nodeOptions.Value.Tracer.ClearStatistics).ToUnixTimeSeconds(),
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                //TODO log error
            }
        }

        private async Task NetworkDiscovery()
        {
            var nodesActive = _nodesRepository.GetNodes(NodesRepositoryFilter.OnlyActive).ToList();
            var nodesActiveWithoutSelf = nodesActive.Where(n => !n.Contains(_nodeOptions.Value.Self)).ToList();
            if (nodesActive.Count == nodesActiveWithoutSelf.Count)
            {
                nodesActive.Add(_nodeOptions.Value.Self);
            }
            var tasks = nodesActiveWithoutSelf.Select(async (nodeUrl) =>
            {
                var timer = DateTimeOffset.Now.ToUnixTimeSeconds();
                var result = await _peerCommunicationService.GetAsync<InformationNodes>(nodeUrl, Endpoints.Information + "/nodes");
                var timespan = DateTimeOffset.Now.ToUnixTimeSeconds() - timer;
                ProcessResult(nodeUrl, result, timespan);
                if (result != null && result.Nodes != null && !result.Nodes.Contains(_nodeOptions.Value.Self))
                {
                    await _peerCommunicationService.SendAsync(
                        nodesActiveWithoutSelf, 
                        Endpoints.Information + "/nodes", 
                        new InformationNodesAdd()
                    {
                        Nodes = nodesActive
                    });
                }
            });
            await Task.WhenAll(tasks);
        }

        private void ProcessResult(string nodeUrl, InformationNodes informationNodes, long delayInSeconds)
        {
            lock (_lock)
            {
                if (informationNodes != null)
                {
                    Console.WriteLine("\tGot result from {0}", nodeUrl);
                    _nodesRepository.AddNodesAsync(
                        informationNodes.Nodes.Where(n => !n.Contains(_nodeOptions.Value.Self))
                        );
                    _nodesRepository.RegisterNodeStatisticAsync(nodeUrl, delayInSeconds, true);
                }
                else
                {
                    _nodesRepository.RegisterNodeStatisticAsync(nodeUrl, delayInSeconds, false);
                }
            }
        }



    }
}
