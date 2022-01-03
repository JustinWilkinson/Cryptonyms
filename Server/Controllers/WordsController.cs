using Cryptonyms.Server.Repository;
using Cryptonyms.Server.Services;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly ILogger<GamesController> _logger;
        private readonly IWordRepository _wordRepository;
        private readonly IProfanityFilter _profanityFilter;

        public WordsController(ILogger<GamesController> logger, IWordRepository wordRepository, IProfanityFilter profanityFilter)
        {
            _logger = logger;
            _wordRepository = wordRepository;
            _profanityFilter = profanityFilter;
        }

        [HttpPost("New")]
        public Task New([FromBody] string word) => _wordRepository.CreateWordAsync(word);

        [HttpGet("Count")]
        public Task<int> Count() => _wordRepository.GetCountAsync();

        [HttpGet]
        public IAsyncEnumerable<EditableWord> List() => _wordRepository.ListWordsAsync();

        [HttpDelete("Delete")]
        public Task Delete(string word) => _wordRepository.DeleteWordAsync(word);

        [HttpPost("ProfanityCheck")]
        public ValueTask<bool> ProfanityCheck([FromBody] string word) => _profanityFilter.ContainsProfanityAsync(word);
    }
}