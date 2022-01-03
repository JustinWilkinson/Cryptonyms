namespace Cryptonyms.Shared
{
    public enum Team
    {
        Red,
        Blue
    }

    public static class TeamExtensions
    {
        public static Team OpposingTeam(this Team team) => team == Team.Red ? Team.Blue : Team.Red;
    }
}