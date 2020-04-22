using Codenames.Server.Extensions;
using Codenames.Server.Repository;
using Codenames.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Codenames.Server.Controllers
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
            var game = Game.NewGame(json.GetStringProperty("GameName"), _wordRepository.ListWords());
            game.Players.AddRange(json.DeserializeStringProperty<IEnumerable<Player>>("Players"));
            _gameRepository.CreateGame(game, json.GetBooleanProperty("PrivateGame"));
            _gameCountRepository.IncrementGameCount();
            return game.GameId;
        }

        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpPost("Save")]
        public void Save(JsonElement gameJson) => _gameRepository.SaveGame(gameJson.Deserialize<Game>());

        [HttpPost("UpdatePlayerInGame")]
        public void UpdatePlayerInGame(JsonElement gameJson)
        {
            var playerInGame = gameJson.Deserialize<PlayerInGame>();
            _gameRepository.AddOrUpdatePlayerInGame(playerInGame.GameId, playerInGame.Player);
        }

        [HttpGet("Count")]
        public int Get() => _gameCountRepository.GetGameCount();
    }
}