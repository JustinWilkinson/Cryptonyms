using Codenames.Server.Extensions;
using Codenames.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Codenames.Server.Repository
{
    public interface IGameRepository
    {
        void CreateGame(Game game);

        void SaveGame(Game game);

        void AddOrUpdatePlayerInGame(string gameId, Player player);

        Game GetGame(string id);

        IEnumerable<Game> ListGames();

        void DeleteGames(IEnumerable<Guid> gameIds);
    }

    public class GameRepository : Repository, IGameRepository
    {
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(ILogger<GameRepository> logger) : base("CREATE TABLE IF NOT EXISTS Games (Id text PRIMARY KEY, GameJson text NOT NULL)")
        {
            _logger = logger;
        }

        public void CreateGame(Game game)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Games (Id, GameJson) VALUES(@Id, @Json)");
                command.AddParameter("@Id", game.GameId);
                command.AddParameter("@Json", game.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating a new game.");
                throw;
            }
        }

        public void SaveGame(Game game)
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id");
                command.AddParameter("@Id", game.GameId);
                command.AddParameter("@Json", game.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred creating a saving game '{game.GameId}'.");
                throw;
            }
        }

        public void AddOrUpdatePlayerInGame(string gameId, Player player)
        {
            try
            {
                ExecuteInTransaction((connection) =>
                {
                    var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
                    selectCommand.AddParameter("@Id", gameId);
                    using var reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        var game = DeserializeColumn<Game>("GameJson")(reader);
                        var existingPlayer = game.Players.SingleOrDefault(p => p.Name == player.Name);
                        if (existingPlayer != null)
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
                _logger.LogError(ex, $"An error occurred creating a identifying player '{player.Name}' in game '{gameId}'.");
                throw;
            }
        }

        public Game GetGame(string id)
        {
            try
            {
                var command = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id");
                command.AddParameter("@Id", id);
                return Execute(command, DeserializeColumn<Game>("GameJson")).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving game '{id}'.");
                throw;
            }
        }

        public IEnumerable<Game> ListGames()
        {
            try
            {
                return Execute("SELECT * FROM Games", DeserializeColumn<Game>("GameJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred listing games.");
                throw;
            }
        }

        public void DeleteGames(IEnumerable<Guid> gameIds)
        {
            try
            {
                ExecuteInTransaction((connection) =>
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
                _logger.LogError(ex, $"An error occurred clearing old games.");
                throw;
            }
        }
    }
}