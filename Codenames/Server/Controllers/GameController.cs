using Codenames.Shared;
using Codenames.Server.Extensions;
using Codenames.Server.Repository;
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
        private readonly IGameRepository _gameRepository;
        private readonly IWordRepository _wordRepository;

        public GameController(ILogger<GameController> logger, IGameRepository gameRepository, IWordRepository wordRepository)
        {
            _logger = logger;
            _gameRepository = gameRepository;
            _wordRepository = wordRepository;
        }

        [HttpPut("New")]
        public Guid New(JsonElement playersJson)
        {
            var game = Game.NewGame(_wordRepository.ListWords());
            game.Players.AddRange(playersJson.Deserialize<IEnumerable<Player>>());
            _gameRepository.CreateGame(game);
            return game.GameId;
        }

        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpPost("Save")]
        public void Save(JsonElement gameJson) => _gameRepository.SaveGame(gameJson.Deserialize<Game>());

        [HttpPost("SaveIdentifiedPlayer")]
        public void SaveIdentifiedPlayer(JsonElement gameJson)
        {
            var playerInGame = gameJson.Deserialize<PlayerInGame>();
            _gameRepository.IdentifyPlayerInGame(playerInGame.GameId, playerInGame.Player);
        }
    }
}