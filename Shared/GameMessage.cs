using System;

namespace Cryptonyms.Shared
{
    public record GameMessage
    {
        public DateTime SentAt { get; set; }

        public string PlayerName { get; set; }

        public string Message { get; set; }

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