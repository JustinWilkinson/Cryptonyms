namespace Cryptonyms.Server.Configuration
{
    public record ApplicationOptions
    {
        public string ConnectionString { get; set; }

        public string SeedWordsPath { get; set; }

        public string ProfanitiesPath { get; set; }
    }
}