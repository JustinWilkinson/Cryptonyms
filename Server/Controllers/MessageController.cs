using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace Cryptonyms.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageRepository _chatRepository;

        public MessageController(ILogger<MessageController> logger, IMessageRepository chatRepository)
        {
            _logger = logger;
            _chatRepository = chatRepository;
        }

        [HttpPut("AddMessage")]
        public void AddMessage(JsonElement json) => _chatRepository.AddMessageAsync(json.GetStringProperty("MessageBoardId"), json.GetObjectProperty<GameMessage>("GameMessage"));

        [HttpGet("GetGameMessagesForGroup")]
        public IAsyncEnumerable<GameMessage> Get(string messageBoardId) => _chatRepository.GetGameMessagesForGroupAsync(messageBoardId);
    }
}