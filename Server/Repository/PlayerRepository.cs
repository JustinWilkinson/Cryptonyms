using Cryptonyms.Server.Extensions;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Cryptonyms.Server.Repository
{
    public interface IPlayerRepository
    {
        void AddPlayer(string deviceId, Player Player);

        void UpdatePlayer(string deviceId, Player Player);

        void ReplacePlayers(string deviceId, IEnumerable<Player> players);

        void DeletePlayer(string deviceId, string name);

        Player GetPlayer(string deviceId, string name);

        IEnumerable<Player> GetPlayers(string deviceId);

        void DeletePlayersForDevices(IEnumerable<string> deviceIds);
    }

    public class PlayerRepository : Repository, IPlayerRepository
    {
        private readonly ILogger<PlayerRepository> _logger;

        public PlayerRepository(ILogger<PlayerRepository> logger) : base("CREATE TABLE IF NOT EXISTS Players (DeviceId text, Name text, PlayerJson text)")
        {
            _logger = logger;
        }

        public void AddPlayer(string deviceId, Player player)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Players (DeviceId, Name, PlayerJson) VALUES (@DeviceId, @Name, @Json)");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", player.Name);
                command.AddParameter("@Json", player.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred creating a new player '{player.Name}' for device '{deviceId}'.");
                throw;
            }
        }

        public void UpdatePlayer(string deviceId, Player player)
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Players SET PlayerJson = @Json WHERE DeviceId = @DeviceId AND Name = @Name");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", player.Name);
                command.AddParameter("@Json", player.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred updating player '{player.Name}' for device '{deviceId}'.");
                throw;
            }
        }

        public Player GetPlayer(string deviceId, string name)
        {
            try
            {
                var command = new SQLiteCommand("SELECT PlayerJson FROM Players WHERE DeviceId = @DeviceId AND Name = @Name");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", name);
                return Execute(command, DeserializeColumn<Player>("PlayerJson")).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving player '{name}' for device '{deviceId}'.");
                throw;
            }
        }

        public IEnumerable<Player> GetPlayers(string deviceId)
        {
            try
            {
                var command = new SQLiteCommand("SELECT PlayerJson FROM Players WHERE DeviceId = @DeviceId");
                command.AddParameter("@DeviceId", deviceId);
                return Execute(command, DeserializeColumn<Player>("PlayerJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving players for device '{deviceId}'.");
                throw;
            }
        }

        public void ReplacePlayers(string deviceId, IEnumerable<Player> players)
        {
            try
            {
                ExecuteInTransaction((connection) =>
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

        public void DeletePlayer(string deviceId, string name)
        {
            try
            {
                var command = new SQLiteCommand("DELETE FROM Players WHERE DeviceId = @DeviceId AND Name = @Name");
                command.AddParameter("@DeviceId", deviceId);
                command.AddParameter("@Name", name);
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred deleting player '{name}' for device '{deviceId}'.");
                throw;
            }
        }

        public void DeletePlayersForDevices(IEnumerable<string> deviceIds)
        {
            try
            {
                ExecuteInTransaction(connection =>
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