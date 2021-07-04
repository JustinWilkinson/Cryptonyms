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
    /// Interface for managing the Players table.
    /// </summary>
    public interface IPlayerRepository
    {
        Task AddPlayerAsync(string deviceId, Player Player);

        Task UpdatePlayerAsync(string deviceId, Player Player);

        Task ReplacePlayersAsync(string deviceId, IEnumerable<Player> players);

        Task DeletePlayerAsync(string deviceId, string name);

        Task<Player> GetPlayerAsync(string deviceId, string name);

        IAsyncEnumerable<Player> GetPlayersAsync(string deviceId);

        Task DeletePlayersForDevicesAsync(IEnumerable<string> deviceIds);
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

        public async Task AddPlayerAsync(string deviceId, Player player)
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
                _logger.LogError(ex, $"An error occurred creating a new player '{player.Name}' for device '{deviceId}'.");
                throw;
            }
        }

        public async Task UpdatePlayerAsync(string deviceId, Player player)
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
                _logger.LogError(ex, $"An error occurred updating player '{player.Name}' for device '{deviceId}'.");
                throw;
            }
        }

        public async Task<Player> GetPlayerAsync(string deviceId, string name)
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
                _logger.LogError(ex, $"An error occurred retrieving player '{name}' for device '{deviceId}'.");
                throw;
            }
        }

        public IAsyncEnumerable<Player> GetPlayersAsync(string deviceId)
        {
            try
            {
                var command = new SQLiteCommand("SELECT PlayerJson FROM Players WHERE DeviceId = @DeviceId");
                command.AddParameter("@DeviceId", deviceId);
                return ExecuteAsync(command, DeserializeColumn<Player>("PlayerJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving players for device '{deviceId}'.");
                throw;
            }
        }

        public async Task ReplacePlayersAsync(string deviceId, IEnumerable<Player> players)
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
                _logger.LogError(ex, $"An error occurred replacing all players for device '{deviceId}'.");
                throw;
            }
        }

        public async Task DeletePlayerAsync(string deviceId, string name)
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
                _logger.LogError(ex, $"An error occurred deleting player '{name}' for device '{deviceId}'.");
                throw;
            }
        }

        public async Task DeletePlayersForDevicesAsync(IEnumerable<string> deviceIds)
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