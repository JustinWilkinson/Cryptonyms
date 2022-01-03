using Cryptonyms.Server.Extensions;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Interface for managing the Players table.
    /// </summary>
    public interface IPlayerRepository
    {
        Task AddPlayer(string deviceId, Player Player);

        Task UpdatePlayer(string deviceId, Player Player);

        Task ReplacePlayers(string deviceId, IEnumerable<Player> players);

        Task DeletePlayer(string deviceId, string name);

        Task<Player> GetPlayer(string deviceId, string name);

        IAsyncEnumerable<Player> GetPlayers(string deviceId);

        Task DeletePlayersForDevices(IEnumerable<string> deviceIds);
    }

    /// <summary>
    /// Manages the players table.
    /// </summary>
    public class PlayerRepository : Repository, IPlayerRepository
    {
        private readonly ILogger<PlayerRepository> _logger;

        protected override string CreateStatement { get; } = "CREATE TABLE IF NOT EXISTS Players (DeviceId text, Name text, PlayerJson text)";

        public PlayerRepository(ILogger<PlayerRepository> logger)
        {
            _logger = logger;
        }

        public async Task AddPlayer(string deviceId, Player player)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Players (DeviceId, Name, PlayerJson) VALUES (@DeviceId, @Name, @Json)");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", player.Name);
                command.AddParameter("@Json", player.Serialize());
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating a new player '{name}' for device '{deviceId}'.", player.Name, deviceId);
                throw;
            }
        }

        public async Task UpdatePlayer(string deviceId, Player player)
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Players SET PlayerJson = @Json WHERE DeviceId = @DeviceId AND Name = @Name");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", player.Name);
                command.AddParameter("@Json", player.Serialize());
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating player '{name}' for device '{deviceId}'.", player.Name, deviceId);
                throw;
            }
        }

        public async Task<Player> GetPlayer(string deviceId, string name)
        {
            try
            {
                var command = new SQLiteCommand("SELECT PlayerJson FROM Players WHERE DeviceId = @DeviceId AND Name = @Name");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", name);
                return await ExecuteAsync(command, DeserializeColumn<Player>("PlayerJson")).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving player '{name}' for device '{deviceId}'.", name, deviceId);
                throw;
            }
        }

        public IAsyncEnumerable<Player> GetPlayers(string deviceId)
        {
            try
            {
                var command = new SQLiteCommand("SELECT PlayerJson FROM Players WHERE DeviceId = @DeviceId");
                command.AddParameter("@DeviceId", deviceId);
                return ExecuteAsync(command, DeserializeColumn<Player>("PlayerJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving players for device '{deviceId}'.", deviceId);
                throw;
            }
        }

        public async Task ReplacePlayers(string deviceId, IEnumerable<Player> players)
        {
            try
            {
                await ExecuteInTransactionAsync((connection) =>
                {
                    var deleteCommand = new SQLiteCommand("DELETE FROM Players WHERE DeviceId = @DeviceId", connection);
                    deleteCommand.AddParameter("@DeviceId", deviceId);
                    deleteCommand.ExecuteNonQuery();
                    foreach (var player in players)
                    {
                        var command = new SQLiteCommand("INSERT INTO Players (DeviceId, Name, PlayerJson) VALUES(@DeviceId, @Name, @Json)", connection);
                        command.AddParameter("@DeviceId", deviceId);
                        command.AddParameter("@Name", player.Name);
                        command.AddParameter("@Json", player.Serialize());
                        command.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred replacing all players for device '{deviceId}'.", deviceId);
                throw;
            }
        }

        public async Task DeletePlayer(string deviceId, string name)
        {
            try
            {
                var command = new SQLiteCommand("DELETE FROM Players WHERE DeviceId = @DeviceId AND Name = @Name");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", name);
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred deleting player '{name}' for device '{deviceId}'.", deviceId);
                throw;
            }
        }

        public async Task DeletePlayersForDevices(IEnumerable<string> deviceIds)
        {
            try
            {
                await ExecuteInTransactionAsync(connection =>
                {
                    foreach (var deviceId in deviceIds)
                    {
                        var command = new SQLiteCommand("DELETE FROM Players WHERE DeviceId = @DeviceId", connection);
                        command.AddParameter("@DeviceId", deviceId);
                        command.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred deleting players on old devices.");
                throw;
            }
        }
    }
}