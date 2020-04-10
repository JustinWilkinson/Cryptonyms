using Codenames.Server.Extensions;
using Codenames.Shared;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Codenames.Server.Repository
{
    public interface IChatRepository
    {
        void AddMessage(string chatId, ChatMessage message);

        IEnumerable<ChatMessage> GetMessagesForChat(string chatId);
    }

    public class ChatRepository : Repository, IChatRepository
    {
        public ChatRepository() : base("CREATE TABLE IF NOT EXISTS ChatMessages (ChatId text, ChatMessageJson text)")
        {

        }

        public void AddMessage(string chatId, ChatMessage message)
        {
            var command = new SQLiteCommand("INSERT INTO ChatMessages (ChatId, ChatMessageJson) VALUES (@ChatId, @Json)");
            command.AddParameter("@ChatId", chatId);
            command.AddParameter("@Json", message.Serialize());
            Execute(command);
        }

        public IEnumerable<ChatMessage> GetMessagesForChat(string chatId)
        {
            var command = new SQLiteCommand("SELECT ChatMessageJson FROM ChatMessages WHERE ChatId = @ChatId");
            command.AddParameter("@ChatId", chatId);
            return Execute(command, DeserializeColumn<ChatMessage>("ChatMessageJson"));
        }
    }
}