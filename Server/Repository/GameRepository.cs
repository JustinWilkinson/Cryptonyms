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
    /// Interface for managing the Games table.
    /// </summary>
    public interface IGameRepository
    {
        Task CreateGameAsync(Game game, bool privateGame);

        Task SaveGameAsync(Game game);

        Task AddOrUpdatePlayerInGameAsync(string gameId, Player player);

        Task<Game> GetGameAsync(string id);

        IAsyncEnumerable<Game> ListGamesAsync(bool includePrivate = false);

        Task DeleteGamesAsync(IEnumerable<Guid> gameIds);
    }

    /// <summary>
    /// Manages the Games table.
    /// </summary>
    public class GameRepository : Repository, IGameRepository
    {
        private readonly ILogger<GameRepository> _logger;

        protected override string CreateStatement { get; } = "CREATE TABLE IF NOT EXISTS Games (Id text PRIMARY KEY, GameJson text NOT NULL, Private integer NOT NULL CHECK (Private IN (0,1)))";

        public GameRepository(ILogger<GameRepository> logger)
        {
            _logger = logger;
        }

        public async Task CreateGameAsync(Game game, bool privateGame)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Games (Id, GameJson, Private) VALUES(@Id, @Json, @Private)");
                command.AddParameter("@Id", game.GameId);
                command.AddParameter("@Json", game.Serialize());
                command.AddParameter("@Private", privateGame ? 1 : 0);
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating a new game.");
                throw;
            }
        }

        public async Task SaveGameAsync(Game game)
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id");
                command.AddParameter("@Id", game.GameId);
                command.AddParameter("@Json", game.Serialize());
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred saving game '{game.GameId}'.");
                throw;
            }
        }

        public async Task AddOrUpdatePlayerInGameAsync(string gameId, Player player)
        {
            try
            {
                await ExecuteInTransactionAsync((connection) =>
                {
                    var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
                    selectCommand.AddParameter("@Id", gameId);
                    using var reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        var game = DeserializeColumn<Game>("GameJson")(reader);
                        var existingPlayer = game.Players.SingleOrDefault(p => p.Name == player.Name);
                        if (existingPlayer is not null)
                        {
                            existingPlayer.Identified = player.Identified;
                        }
                        else
                        {
                            game.Players.Add(player);
                        }
                        var updateCommand = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id", connection);
                        updateCommand.AddParameter("@Id", game.GameId);
                        updateCommand.AddParameter("@Json", game.Serialize());
                        updateCommand.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred identifying player '{player.Name}' in game '{gameId}'.");
                throw;
            }
        }

        public async Task<Game> GetGameAsync(string id)
        {
            try
            {
                var command = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id");
                command.AddParameter("@Id", id);
                return await ExecuteAsync(command, DeserializeColumn<Game>("GameJson")).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving game '{id}'.");
                throw;
            }
        }

        public IAsyncEnumerable<Game> ListGamesAsync(bool includePrivate = false)
        {
            try
            {
                return ExecuteAsync($"SELECT * FROM Games{(includePrivate ? "" : " WHERE Private = 0")}", DeserializeColumn<Game>("GameJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred listing games.");
                throw;
            }
        }

        public async Task DeleteGamesAsync(IEnumerable<Guid> gameIds)
        {
            try
            {
                await ExecuteInTransactionAsync((connection) =>
                {
                    foreach (var gameId in gameIds)
                    {
                        var command = new SQLiteCommand("DELETE FROM Games WHERE Id = @Id", connection);
                        command.AddParameter("@Id", gameId);
                        command.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred clearing old games.");
                throw;
            }
        }
    }
}