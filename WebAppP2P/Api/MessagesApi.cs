using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppP2P.WebSockets.InternalMessages;
using WebAppP2P.Dto;
using WebAppP2P.Core.Messages;
using WebAppP2P.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppP2P.Api
{
    public class MessagesApi : Controller
    {
        private readonly IMessageStore _messageStore;
        private readonly IInternalMessageMediator _internalMessageManager;
        private readonly IPeerCommunicationService _peerCommunicationService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public MessagesApi(IMessageStore messageManager, 
            IInternalMessageMediator internalMessageManager,
            IPeerCommunicationService peerCommunicationService,
            ILogger<MessagesApi> logger,
            IMapper mapper
            )
        {
            _messageStore = messageManager;
            _internalMessageManager = internalMessageManager;
            _peerCommunicationService = peerCommunicationService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("api/messages")]
        public async Task<IActionResult> PostAsync([FromBody] EncryptedMessage message)
        {
            _logger.LogInformation("Got message at endpoint api/messages from {0}:{1}", HttpContext.Connection.RemoteIpAddress, HttpContext.Connection.RemotePort);
            var result = _messageStore.TryAddAsync(message);
            if (!result.Result)
            {
                return BadRequest();
            }

            await _internalMessageManager.SendAsync(message);
            await _peerCommunicationService.SendAsync(Endpoints.Messages, message);
            return Ok();
        }

        [HttpGet("api/messages/{id}")]
        public IActionResult Get(string id)
        {
            var idDecoded = UrlUtils.UrlDecodeWithBase64(id);
            EncryptedMessage message;
            var result = _messageStore.TryGet(idDecoded, out message);
            if (!result)
            {
                return NotFound();
            }
            return Ok(message);
        }

        [HttpGet("api/messages")]
        public IActionResult Get()
        {
            return Ok(_messageStore.GetAll());
        }
    }
}
