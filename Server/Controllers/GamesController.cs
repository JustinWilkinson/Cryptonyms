using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ILogger<GamesController> _logger;
        private readonly IGameCountRepository _gameCountRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IWordRepository _wordRepository;

        public GamesController(ILogger<GamesController> logger, IGameCountRepository gameCountRepository, IGameRepository gameRepository, IWordRepository wordRepository)
        {
            _logger = logger;
            _gameCountRepository = gameCountRepository;
            _gameRepository = gameRepository;
            _wordRepository = wordRepository;
        }

        [HttpPost]
        public async Task<Guid> New(GameConfiguration gameConfiguration)
        {
            var words = await _wordRepository.ListWordsAsync().SelectAsync(w => w.Text).ToEnumerableAsync();
            var game = Game.NewGame(gameConfiguration, words);
            await _gameRepository.CreateGameAsync(game, gameConfiguration.PrivateGame);
            await _gameCountRepository.IncrementGameCountAsync();
            return game.GameId;
        }

        // Needs to return a string as System.Text.Json.JsonSerializer cannot handle the Dictionary<int, string> of words.
        [HttpGet("{id}")]
        public async Task<string> Get([FromRoute] string id) => (await _gameRepository.GetGameAsync(id)).Serialize();

        // Needs to return a string as System.Text.Json.JsonSerializer cannot handle the Dictionary<int, string> of words.
        [HttpGet]
        public async Task<string> List() => (await _gameRepository.ListGamesAsync().ToEnumerableAsync()).Serialize();

        [HttpPut("Save")]
        public Task Save(JsonElement gameJson) => _gameRepository.SaveGameAsync(gameJson.Deserialize<Game>());

        [HttpPatch("{id}/UpdatePlayer")]
        public Task UpdatePlayerInGame([FromRoute] string id, [FromBody] Player player) => _gameRepository.AddOrUpdatePlayerInGameAsync(id, player);

        [HttpGet("Count")]
        public Task<int> Count() => _gameCountRepository.GetGameCountAsync();
    }
}