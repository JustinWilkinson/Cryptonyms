using Cryptonyms.Server.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Cryptonyms.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IWordRepository _wordRepository;

        public WordController(ILogger<GameController> logger, IWordRepository wordRepository)
        {
            _logger = logger;
            _wordRepository = wordRepository;
        }

        [HttpPut("New")]
        public void New(JsonElement word) => _wordRepository.CreateWord(word.GetString());

        [HttpGet("Count")]
        public int Count() => _wordRepository.GetCount();

        [HttpGet("List")]
        public IEnumerable<string> List() => _wordRepository.ListWords();

        [HttpPost("Save")]
        public void Save(JsonElement wordsArray)
        {
            var words = wordsArray.EnumerateArray().Select(w => w.GetString()).ToArray();
            if (words.Length == 2)
            {
                _wordRepository.EditWord(words[0], words[1]);
            }
        }

        [HttpDelete("Delete")]
        public void Delete(string word) => _wordRepository.DeleteWord(word);
    }
}