namespace Cryptonyms.Shared
{
    public record Turn
    {
        public Team Team { get; init; }

        public int NumberOfGuesses { get; set; }

        public int NumberOfWordsLinked { get; set; }

        public string Clue { get; set; }
    }
}