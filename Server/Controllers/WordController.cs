using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Repository;
using Cryptonyms.Server.Services;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IWordRepository _wordRepository;
        private readonly IProfanityFilter _profanityFilter;

        public WordController(ILogger<GameController> logger, IWordRepository wordRepository, IProfanityFilter profanityFilter)
        {
            _logger = logger;
            _wordRepository = wordRepository;
            _profanityFilter = profanityFilter;
        }

        [HttpPut("New")]
        public Task New(JsonElement word) => _wordRepository.CreateWordAsync(word.GetString());

        [HttpGet("Count")]
        public Task<int> Count() => _wordRepository.GetCountAsync();

        [HttpGet("List")]
        public IAsyncEnumerable<EditableWord> List() => _wordRepository.ListWordsAsync();

        [HttpDelete("Delete")]
        public Task Delete(string word) => _wordRepository.DeleteWordAsync(word);

        [HttpPost("ProfanityCheck")]
        public ValueTask<bool> ProfanityCheck(JsonElement json) => _profanityFilter.ContainsProfanityAsync(json.GetStringProperty("Word"));
    }
}