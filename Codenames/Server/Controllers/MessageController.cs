using Codenames.Server.Extensions;
using Codenames.Server.Repository;
using Codenames.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Codenames.Server.Controllers
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
        public void AddMessage(JsonElement json) => _chatRepository.AddMessage(json.GetStringProperty("MessageBoardId"), json.DeserializeStringProperty<GameMessage>("GameMessage"));

        [HttpGet("GetGameMessagesForGroup")]
        public string Get(string MessageBoardId) => _chatRepository.GetGameMessagesForGroup(MessageBoardId).Serialize();
    }
}