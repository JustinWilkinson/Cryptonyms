using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptonyms.Shared
{
    public record Game
    {
        private static readonly Random _random = new();

        public Guid GameId { get; set; }

        public string Name { get; set; }

        public Dictionary<int, string> Words { get; set; }

        public int Assassin { get; set; }

        public List<int> RedWords { get; set; }

        public List<int> BlueWords { get; set; }

        public string CompletedMessage { get; set; }

        public DateTime StartedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        public Team? WinnningTeam { get; set; }

        public List<Player> Players { get; set; }

        public Turn CurrentTurn { get; set; }

        public List<int> GuessedWords { get; set; }

        public static Game NewGame(string name, IEnumerable<string> knownWords)
        {
            var knownWordsArray = knownWords.ToArray();
            var allPossibilities = Enumerable.Range(0, knownWordsArray.Length - 1).ToList();
            Game game = new()
            {
                GameId = Guid.NewGuid(),
                Name = name ?? "Unnamed Game",
                StartedAtUtc = DateTime.UtcNow,
                Words = new Dictionary<int, string>(),
                RedWords = new List<int>(),
                BlueWords = new List<int>(),
                Players = new List<Player>(),
                CurrentTurn = new Turn(),
                GuessedWords = new List<int>()
            };

            for (int i = 0; i < 25; i++)
            {
                game.Words.Add(i, knownWordsArray[GetNextWordIndex(allPossibilities)]);
            }

            var redGoesFirst = _random.NextDouble() > 0.5;
            var counts = redGoesFirst ? new { RedCount = 9, BlueCount = 8 } : new { RedCount = 8, BlueCount = 9 };
            game.CurrentTurn.Team = redGoesFirst ? Team.Red : Team.Blue;

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
            var nextIndex = _random.Next(0, possibilities.Count);
            var nextWordIndex = possibilities[nextIndex];
            possibilities.RemoveAt(nextIndex);
            return nextWordIndex;
        }
    }
}