using System;

namespace Cryptonyms.Shared
{
    public record GameMessage
    {
        public DateTime SentAt { get; init; }

        public string PlayerName { get; init; }

        public string Message { get; init; }

        [Obsolete("For use by the serializer only", true)]
        public GameMessage()
        {
        }

        public GameMessage(string playerName, string message)
        {
            SentAt = DateTime.UtcNow;
            PlayerName = playerName;
            Message = message;
        }
    }
}