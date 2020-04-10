using Codenames.Server.Extensions;
using Codenames.Shared;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Codenames.Server.Repository
{
    public interface IPlayerRepository
    {
        void AddPlayer(string deviceId, Player Player);

        void UpdatePlayer(string deviceId, Player Player);

        void ReplacePlayers(string deviceId, IEnumerable<Player> players);

        void DeletePlayer(string deviceId, string name);

        Player GetPlayer(string deviceId, string name);

        IEnumerable<Player> GetPlayers(string deviceId);
    }

    public class PlayerRepository : Repository, IPlayerRepository
    {
        public PlayerRepository() : base("CREATE TABLE IF NOT EXISTS Players (DeviceId text, Name text, PlayerJson text)")
        {

        }

        public void AddPlayer(string deviceId, Player player)
        {
            var command = new SQLiteCommand("INSERT INTO Players (DeviceId, Name, PlayerJson) VALUES (@DeviceId, @Name, @Json)");
            command.AddParameter("@DeviceId", deviceId);
            command.AddParameter("@Name", player.Name);
            command.AddParameter("@Json", player.Serialize());
            Execute(command);
        }

        public void UpdatePlayer(string deviceId, Player player)
        {
            var command = new SQLiteCommand("UPDATE Players SET PlayerJson = @Json WHERE DeviceId = @DeviceId AND Name = @Name");
            command.AddParameter("@DeviceId", deviceId);
            command.AddParameter("@Name", player.Name);
            command.AddParameter("@Json", player.Serialize());
            Execute(command);
        }

        public Player GetPlayer(string deviceId, string name)
        {
            var command = new SQLiteCommand("SELECT PlayerJson FROM Players WHERE DeviceId = @DeviceId AND Name = @Name");
            command.AddParameter("@DeviceId", deviceId);
            command.AddParameter("@Name", name);
            return Execute(command, DeserializeColumn<Player>("PlayerJson")).SingleOrDefault();
        }

        public IEnumerable<Player> GetPlayers(string deviceId)
        {
            var command = new SQLiteCommand("SELECT PlayerJson FROM Players WHERE DeviceId = @DeviceId");
            command.AddParameter("@DeviceId", deviceId);
            return Execute(command, DeserializeColumn<Player>("PlayerJson"));
        }

        public void ReplacePlayers(string deviceId, IEnumerable<Player> players)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();
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
            transaction.Commit();
        }

        public void DeletePlayer(string deviceId, string name)
        {
            var command = new SQLiteCommand("DELETE FROM Players WHERE DeviceId = @DeviceId AND Name = @Name");
            command.AddParameter("@DeviceId", deviceId);
            command.AddParameter("@Name", name);
            Execute(command);
        }
    }
}