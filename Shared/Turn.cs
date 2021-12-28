namespace Cryptonyms.Shared
{
    public record Turn
    {
        public Team Team { get; set; }

        public int NumberOfGuesses { get; set; }

        public int InitialNumberOfGuesses { get; set; }

        public string Clue { get; set; }
    }
}