using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Nodes;

namespace WebAppP2P.Services
{

    public class PeerCommunicationService : IPeerCommunicationService
    {
        private readonly INodesRepository _nodesRepository;  
        private readonly ILogger<PeerCommunicationService> _logger;

        public PeerCommunicationService(INodesRepository nodesRepository,
            ILogger<PeerCommunicationService> logger)
        {
            _nodesRepository = nodesRepository;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string endpointWithArgs)
        { 
            foreach (var nodeUrl in _nodesRepository.GetNodes(NodesRepositoryFilter.OnlyActive))
            {
                var obj = await GetAsync<T>(nodeUrl, endpointWithArgs);
            }
            return default(T);
        }

        public async Task<T> GetAsync<T>(string nodeUrl, string endpointWithArgs)
        {
            var httpClient = new HttpClient();
            var fullUrl = nodeUrl.EndsWith("/") ? nodeUrl + endpointWithArgs : nodeUrl + "/" + endpointWithArgs;
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            try
            {
                var result = await httpClient.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    var body = await result.Content.ReadAsStringAsync();

                    var obj = JsonConvert.DeserializeObject<T>(body);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            catch (HttpRequestException) { }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            _logger.LogInformation("Cannot get from {0}", fullUrl);
            return default(T);
        }

        public async Task SendAsync(string endpoint, object objectToSend)
        {
            var nodes = _nodesRepository
                .GetNodes(NodesRepositoryFilter.OnlyActive);
            await SendAsync(nodes, endpoint, objectToSend);
        }

        public async Task SendAsync(IEnumerable<string> nodes, string endpoint, object objectToSend)
        {
            var httpClient = new HttpClient();
            var tasks = nodes
                .Select(async (nodeUrl) =>
                {
                    var fullUrl = nodeUrl.EndsWith("/") ? nodeUrl + endpoint : nodeUrl + "/" + endpoint;
                    var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
                    {
                        Content = new StringContent(
                            JsonConvert.SerializeObject(objectToSend),
                            Encoding.UTF8,
                            "application/json"
                            )
                    };
                    try
                    {
                        var result = await httpClient.SendAsync(request);
                        _logger.LogInformation("Sync completed with {0} with result {1}", fullUrl, result.StatusCode);
                    }
                    catch (HttpRequestException)
                    {
                        _logger.LogError("Cannot sync with {0}", fullUrl);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogError("Cannot sync with {0} because of task cancel exception", fullUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                });
            await Task.WhenAll(tasks);
        }
    }
}
