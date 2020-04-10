using System;

namespace Codenames.Shared
{
    public class ChatMessage
    {
        public DateTime SentAt { get; set; }

        public string PlayerName { get; set; }

        public string Message { get; set; }

        public ChatMessage()
        {

        }

        public ChatMessage(string playerName, string message)
        {
            SentAt = DateTime.UtcNow;
            PlayerName = playerName;
            Message = message;
        }
    }
}