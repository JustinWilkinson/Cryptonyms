using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameCountRepository _gameCountRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IWordRepository _wordRepository;

        public GameController(ILogger<GameController> logger, IGameCountRepository gameCountRepository, IGameRepository gameRepository, IWordRepository wordRepository)
        {
            _logger = logger;
            _gameCountRepository = gameCountRepository;
            _gameRepository = gameRepository;
            _wordRepository = wordRepository;
        }

        [HttpPut("New")]
        public async Task<Guid> New(JsonElement json)
        {
            var game = Game.NewGame(json.GetStringProperty("GameName"), (await _wordRepository.ListWordsAsync().SelectAsync(w => w.Text).ToEnumerableAsync()));
            game.Players.AddRange(json.GetObjectProperty<IEnumerable<Player>>("Players"));
            await _gameRepository.CreateGameAsync(game, json.GetBooleanProperty("PrivateGame"));
            await _gameCountRepository.IncrementGameCountAsync();
            return game.GameId;
        }

        // Needs to return a string as System.Text.Json.JsonSerializer cannot handle the Dictionary<int, string> of words.
        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGameAsync(id).Serialize();

        // Needs to return a string as System.Text.Json.JsonSerializer cannot handle the Dictionary<int, string> of words.
        [HttpGet("List")]
        public async Task<string> List() => (await _gameRepository.ListGamesAsync().ToEnumerableAsync()).Serialize();

        [HttpPost("Save")]
        public Task Save(JsonElement gameJson) => _gameRepository.SaveGameAsync(gameJson.Deserialize<Game>());

        [HttpPost("UpdatePlayerInGame")]
        public Task UpdatePlayerInGame(JsonElement gameJson) => _gameRepository.AddOrUpdatePlayerInGameAsync(gameJson.GetStringProperty("GameId"), gameJson.GetObjectProperty<Player>("Player"));

        [HttpGet("Count")]
        public Task<int> Count() => _gameCountRepository.GetGameCountAsync();
    }
}