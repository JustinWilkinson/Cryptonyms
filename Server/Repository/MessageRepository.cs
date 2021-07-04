using Cryptonyms.Server.Extensions;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Interface for managing the Messages table.
    /// </summary>
    public interface IMessageRepository
    {
        Task AddMessageAsync(string messageBoardId, GameMessage message);

        IAsyncEnumerable<GameMessage> GetGameMessagesForGroupAsync(string chatId);

        Task DeleteMessagesForGamesAsync(IEnumerable<Guid> gameIds);
    }

    public class MessageRepository : Repository, IMessageRepository
    {
        private readonly ILogger<MessageRepository> _logger;

        protected override string CreateStatement { get; } = "CREATE TABLE IF NOT EXISTS Messages (MessageBoardId text, MessageJson text);";

        public MessageRepository(ILogger<MessageRepository> logger)
        {
            _logger = logger;
        }

        public async Task AddMessageAsync(string messageBoardId, GameMessage message)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Messages (MessageBoardId, MessageJson) VALUES (@MessageBoardId, @Json)");
                command.AddParameter("@MessageBoardId", messageBoardId);
                command.AddParameter("@Json", message.Serialize());
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred inserting a message '{message}' for board '{messageBoardId}'.");
                throw;
            }
        }

        public IAsyncEnumerable<GameMessage> GetGameMessagesForGroupAsync(string messageBoardId)
        {
            try
            {
                var command = new SQLiteCommand("SELECT MessageJson FROM Messages WHERE MessageBoardId = @MessageBoardId");
                command.AddParameter("@MessageBoardId", messageBoardId);
                return ExecuteAsync(command, DeserializeColumn<GameMessage>("MessageJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving messages for board '{messageBoardId}'.");
                throw;
            }
        }


        public async Task DeleteMessagesForGamesAsync(IEnumerable<Guid> gameIds)
        {
            try
            {
                var gameIdsAsStrings = gameIds.Select(x => x.ToString()).ToList();
                var messageBoardIdsToDelete = new List<string>();
                await foreach (var messageBoardId in ExecuteAsync("SELECT DISTINCT MessageBoardId FROM Messages", x => x.ToString()))
                {
                    foreach (var gameId in gameIdsAsStrings)
                    {
                        if (messageBoardId.StartsWith(gameId))
                        {
                            messageBoardIdsToDelete.Add(messageBoardId);
                        }
                    }
                }

                await ExecuteInTransactionAsync((connection) =>
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