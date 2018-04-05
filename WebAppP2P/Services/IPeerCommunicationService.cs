using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAppP2P.Services
{
    public interface IPeerCommunicationService
    {
        Task SendAsync(string endpoint, object objectToSend);
        Task SendAsync(IEnumerable<string> nodes, string endpoint, object objectToSend);
        Task<T> GetAsync<T>(string endpointWithArgs);
        Task<T> GetAsync<T>(string nodeUrl, string endpointWithArgs);
    }
}