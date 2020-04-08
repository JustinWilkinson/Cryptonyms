using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Codenames.Server.Repository
{
    public abstract class Repository
    {
        public const string Database = "Codenames.sqlite";
        public const string ConnectionString = "DataSource=Codenames.sqlite";

        static Repository()
        {
            if (!File.Exists(Database))
            {
                SQLiteConnection.CreateFile(Database);
            }
        }

        protected Repository(string createTable)
        {
            using var connection = GetOpenConnection();
            var command = new SQLiteCommand(createTable, connection);
            command.ExecuteNonQuery();
        }

        protected SQLiteConnection GetOpenConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        protected void Execute(string commandString) => Execute(new SQLiteCommand(commandString));

        protected void Execute(SQLiteCommand command)
        {
            using var connection = GetOpenConnection();
            command.Connection = connection;
            command.ExecuteNonQuery();
        }

        protected T ExecuteScalar<T>(string commandString, Func<object, T> converter = null) => ExecuteScalar(new SQLiteCommand(commandString), converter);

        protected T ExecuteScalar<T>(SQLiteCommand command, Func<object, T> converter = null)
        {
            using var connection = GetOpenConnection();
            command.Connection = connection;
            var scalar = command.ExecuteScalar();
            return converter != null ? converter(scalar) : (T)scalar;
        }

        protected IEnumerable<T> Execute<T>(string commandString, Func<SQLiteDataReader, T> converter) => Execute(new SQLiteCommand(commandString), converter);

        protected IEnumerable<T> Execute<T>(SQLiteCommand command, Func<SQLiteDataReader, T> converter)
        {
            using var connection = GetOpenConnection();
            command.Connection = connection;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return converter(reader);
            }
        }
    }
}