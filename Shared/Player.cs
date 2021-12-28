namespace Cryptonyms.Shared
{
    public record Player
    {
        public string Name { get; set; }

        public bool IsSpymaster { get; set; }

        public Team? Team { get; set; }

        public bool Identified { get; set; }
    }
}