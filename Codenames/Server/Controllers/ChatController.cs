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
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatRepository _chatRepository;

        public ChatController(ILogger<ChatController> logger, IChatRepository chatRepository)
        {
            _logger = logger;
            _chatRepository = chatRepository;
        }

        [HttpPut("AddMessage")]
        public void AddMessage(JsonElement json) => _chatRepository.AddMessage(json.GetStringProperty("ChatId"), json.DeserializeStringProperty<ChatMessage>("ChatMessage"));

        [HttpGet("GetMessagesForChat")]
        public string Get(string chatId) => _chatRepository.GetMessagesForChat(chatId).Serialize();
    }
}