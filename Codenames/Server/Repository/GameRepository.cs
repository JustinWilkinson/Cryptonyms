using Codenames.Server.Extensions;
using Codenames.Shared;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Codenames.Server.Repository
{
    public interface IGameRepository
    {
        void CreateGame(Game game);

        void SaveGame(Game game);

        void IdentifyPlayerInGame(string gameId, Player player);

        Game GetGame(string id);

        IEnumerable<Game> ListGames();
    }

    public class GameRepository : Repository, IGameRepository
    {
        public GameRepository() : base("CREATE TABLE IF NOT EXISTS Games (Id text, GameJson text)")
        {

        }

        public void CreateGame(Game game)
        {
            var command = new SQLiteCommand("INSERT INTO Games (Id, GameJson) VALUES(@Id, @Json)");
            command.AddParameter("@Id", game.GameId);
            command.AddParameter("@Json", game.Serialize());
            Execute(command);
        }

        public void SaveGame(Game game)
        {
            var command = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id");
            command.AddParameter("@Id", game.GameId);
            command.AddParameter("@Json", game.Serialize());
            Execute(command);
        }

        public void IdentifyPlayerInGame(string gameId, Player player)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
            var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
            selectCommand.AddParameter("@Id", gameId);
            using (var reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    game.Players.SingleOrDefault(p => p.Name == player.Name).Identified = true;
                    var updateCommand = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id", connection);
                    updateCommand.AddParameter("@Id", game.GameId);
                    updateCommand.AddParameter("@Json", game.Serialize());
                    updateCommand.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }

        public Game GetGame(string id)
        {
            var command = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id");
            command.AddParameter("@Id", id);
            return Execute(command, DeserializeColumn<Game>("GameJson")).SingleOrDefault();
        }

        public IEnumerable<Game> ListGames() => Execute("SELECT * FROM Games", DeserializeColumn<Game>("GameJson"));
    }
}