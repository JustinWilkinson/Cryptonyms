using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptonyms.Shared
{
    public record Game
    {
        private static readonly Random Random = new();

        public Guid GameId { get; init; }

        public string Name { get; init; }

        public Dictionary<int, string> Words { get; init; }

        public int Assassin { get; private set; }

        public List<int> RedWords { get; init; }

        public List<int> BlueWords { get; init; }

        public string CompletedMessage { get; set; }

        public DateTime StartedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Team? WinningTeam { get; set; }

        public List<Player> Players { get; init; }

        public Turn CurrentTurn { get; set; }

        public List<int> GuessedWords { get; init; }

        public bool AllowMultipleBonusGuesses { get; init; }

        public bool AllowZeroLinks { get; init; }

        public static Game NewGame(GameConfiguration gameConfiguration, IEnumerable<string> knownWords)
        {
            var knownWordsArray = knownWords.ToArray();
            var allPossibilities = Enumerable.Range(0, knownWordsArray.Length - 1).ToList();
            var game = new Game
            {
                GameId = Guid.NewGuid(),
                Name = gameConfiguration.Name ?? "Unnamed Game",
                StartedAtUtc = DateTime.UtcNow,
                Words = new Dictionary<int, string>(),
                RedWords = new List<int>(),
                BlueWords = new List<int>(),
                Players = gameConfiguration.Players.ToList(),
                GuessedWords = new List<int>(),
                AllowMultipleBonusGuesses = gameConfiguration.AllowMultipleBonusGuesses,
                AllowZeroLinks = gameConfiguration.AllowZeroLinks
            };

            for (int i = 0; i < 25; i++)
            {
                game.Words.Add(i, knownWordsArray[GetNextWordIndex(allPossibilities)]);
            }

            var redGoesFirst = Random.NextDouble() > 0.5;
            var counts = redGoesFirst ? new { RedCount = 9, BlueCount = 8 } : new { RedCount = 8, BlueCount = 9 };
            game.CurrentTurn = new Turn { Team = redGoesFirst ? Team.Red : Team.Blue };

            var selectedWords = Enumerable.Range(0, 24).ToList();
            for (var i = 0; i < counts.RedCount; i++)
            {
                game.RedWords.Add(GetNextWordIndex(selectedWords));
            }

            for (var i = 0; i < counts.BlueCount; i++)
            {
                game.BlueWords.Add(GetNextWordIndex(selectedWords));
            }

            game.Assassin = GetNextWordIndex(selectedWords);

            return game;
        }

        private static int GetNextWordIndex(List<int> possibilities)
        {
            var nextIndex = Random.Next(0, possibilities.Count);
            var nextWordIndex = possibilities[nextIndex];
            possibilities.RemoveAt(nextIndex);
            return nextWordIndex;
        }
    }
}