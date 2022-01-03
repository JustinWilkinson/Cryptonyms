using System.Collections.Generic;

namespace Cryptonyms.Shared
{
    public record GameConfiguration
    {
        public string Name { get; init; }

        public bool PrivateGame { get; init; }

        public bool AllowMultipleBonusGuesses { get; init; }

        public bool AllowZeroLinks { get; init; }

        public IEnumerable<Player> Players { get; init; }
    }
}
