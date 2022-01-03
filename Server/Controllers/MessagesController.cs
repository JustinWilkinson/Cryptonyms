using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IMessageRepository _chatRepository;

        public MessagesController(ILogger<MessagesController> logger, IMessageRepository chatRepository)
        {
            _logger = logger;
            _chatRepository = chatRepository;
        }

        [HttpPost("{messageBoardId}")]
        public Task AddMessage([FromRoute] string messageBoardId, [FromBody] GameMessage message) => _chatRepository.AddMessageAsync(messageBoardId, message);

        [HttpGet("{messageBoardId}")]
        public IAsyncEnumerable<GameMessage> ListMessages([FromRoute] string messageBoardId) => _chatRepository.GetGameMessagesForGroupAsync(messageBoardId);
    }
}