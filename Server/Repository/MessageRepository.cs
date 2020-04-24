using Cryptonyms.Server.Extensions;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Cryptonyms.Server.Repository
{
    public interface IMessageRepository
    {
        void AddMessage(string messageBoardId, GameMessage message);

        IEnumerable<GameMessage> GetGameMessagesForGroup(string chatId);

        void DeleteMessagesForGames(IEnumerable<Guid> gameIds);
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


        public void DeleteMessagesForGames(IEnumerable<Guid> gameIds)
        {
            try
            {
                var gameIdsAsStrings = gameIds.Select(x => x.ToString()).ToList();
                var messageBoardIdsToDelete = new List<string>();
                foreach (var messageBoardId in Execute("SELECT DISTINCT MessageBoardId FROM Messages", x => x.ToString()))
                {
                    foreach (var gameId in gameIdsAsStrings)
                    {
                        if (messageBoardId.StartsWith(gameId))
                        {
                            messageBoardIdsToDelete.Add(messageBoardId);
                        }
                    }
                }

                ExecuteInTransaction((connection) =>
                {
                    foreach (var messageBoardId in messageBoardIdsToDelete)
                    {
                        var command = new SQLiteCommand("DELETE FROM Messages WHERE MessageBoardId = @MessageBoardId", connection);
                        command.AddParameter("@MessageBoardId", messageBoardId);
                        command.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred clearing messages for old games.");
                throw;
            }
        }
    }
}