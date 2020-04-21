using Codenames.Server.Extensions;
using Codenames.Shared;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(ILogger<MessageRepository> logger) : base("CREATE TABLE IF NOT EXISTS Messages (MessageBoardId text, MessageJson text);")
        {
            _logger = logger;
        }

        public void AddMessage(string messageBoardId, GameMessage message)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Messages (MessageBoardId, MessageJson) VALUES (@MessageBoardId, @Json)");
                command.AddParameter("@MessageBoardId", messageBoardId);
                command.AddParameter("@Json", message.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred inserting a message '{message}' for board '{messageBoardId}'.");
                throw;
            }
        }

        public IEnumerable<GameMessage> GetGameMessagesForGroup(string messageBoardId)
        {
            try
            {
                var command = new SQLiteCommand("SELECT MessageJson FROM Messages WHERE MessageBoardId = @MessageBoardId");
                command.AddParameter("@MessageBoardId", messageBoardId);
                return Execute(command, DeserializeColumn<GameMessage>("MessageJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving messages for board '{messageBoardId}'.");
                throw;
            }
        }
    }
}