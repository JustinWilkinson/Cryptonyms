using Codenames.Server.Extensions;
using Codenames.Shared;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Codenames.Server.Repository
{
    public interface IMessageRepository
    {
        void AddMessage(string MessageBoardId, GameMessage message);

        IEnumerable<GameMessage> GetGameMessagesForGroup(string chatId);
    }

    public class MessageRepository : Repository, IMessageRepository
    {
        public MessageRepository() : base("CREATE TABLE IF NOT EXISTS Messages (MessageBoardId text, MessageJson text)")
        {

        }

        public void AddMessage(string MessageBoardId, GameMessage message)
        {
            var command = new SQLiteCommand("INSERT INTO Messages (MessageBoardId, MessageJson) VALUES (@MessageBoardId, @Json)");
            command.AddParameter("@MessageBoardId", MessageBoardId);
            command.AddParameter("@Json", message.Serialize());
            Execute(command);
        }

        public IEnumerable<GameMessage> GetGameMessagesForGroup(string MessageBoardId)
        {
            var command = new SQLiteCommand("SELECT MessageJson FROM Messages WHERE MessageBoardId = @MessageBoardId");
            command.AddParameter("@MessageBoardId", MessageBoardId);
            return Execute(command, DeserializeColumn<GameMessage>("MessageJson"));
        }
    }
}