﻿using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        public IEnumerable<EditableWord> List() => _wordRepository.ListWords();

        [HttpDelete("Delete")]
        public void Delete(string word) => _wordRepository.DeleteWord(word);
    }
}