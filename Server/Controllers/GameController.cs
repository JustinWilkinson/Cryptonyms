using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
        public Guid New(JsonElement json)
        {
            var game = Game.NewGame(json.GetStringProperty("GameName"), _wordRepository.ListWords().Select(w => w.Text));
            game.Players.AddRange(json.GetObjectProperty<IEnumerable<Player>>("Players"));
            _gameRepository.CreateGame(game, json.GetBooleanProperty("PrivateGame"));
            _gameCountRepository.IncrementGameCount();
            return game.GameId;
        }

        // Needs to return a string as System.Text.Json.JsonSerializer cannot handle the Dictionary<int, string> of words.
        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        // Needs to return a string as System.Text.Json.JsonSerializer cannot handle the Dictionary<int, string> of words.
        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpPost("Save")]
        public void Save(JsonElement gameJson) => _gameRepository.SaveGame(gameJson.Deserialize<Game>());

        [HttpPost("UpdatePlayerInGame")]
        public void UpdatePlayerInGame(JsonElement gameJson) => _gameRepository.AddOrUpdatePlayerInGame(gameJson.GetStringProperty("GameId"), gameJson.GetObjectProperty<Player>("Player"));

        [HttpGet("Count")]
        public int Get() => _gameCountRepository.GetGameCount();
    }
}