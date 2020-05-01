using Cryptonyms.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Base class for database interactions.
    /// </summary>
    public abstract class Repository
    {
        private static string _connectionString;

        /// <summary>
        /// Creates the database if it doesn't exist, and stores the connection string in memory to limit hits on config file.
        /// </summary>
        /// <param name="connectionString"></param>
        public static void CreateDatabase(string connectionString)
        {
            _connectionString = connectionString;
            var path = new SQLiteConnectionStringBuilder(_connectionString).DataSource;
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }
        }

        /// <summary>
        /// Intended to allow inheriting repositories to remove boilerplate, use this to create the database table for the repository.
        /// </summary>
        /// <param name="createTable">Command to run on instantiation</param>
        protected Repository(string createTable)
        {
            using var connection = GetOpenConnection();
            var command = new SQLiteCommand(createTable, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns an open SQLiteConnection.
        /// </summary>
        protected SQLiteConnection GetOpenConnection()
        {
            var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            return connection;
        }


        /// <summary>
        /// Creates a SQLiteCommand from the provided string and executes it using a new connection.
        /// </summary>
        /// <param name="commandString">Command text to execute</param>
        protected void Execute(string commandString) => Execute(new SQLiteCommand(commandString));

        /// <summary>
        /// Executes the provided command using a new connection.
        /// </summary>
        /// <param name="command">Command to execute</param>
        protected void Execute(SQLiteCommand command)
        {
            using var connection = GetOpenConnection();
            command.Connection = connection;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a SQLiteCommand from the provided string and executes it using a new connection, returning a scalar value using the specified converter.
        /// </summary>
        /// <typeparam name="T">Type to return the scalar as</typeparam>
        /// <param name="commandString">Command text to execute</param>
        /// <param name="converter">Conversion method for returned value</param>
        /// <returns>An instance of T created by converting the returned value with the specified converter.</returns>
        protected T ExecuteScalar<T>(string commandString, Func<object, T> converter = null) => ExecuteScalar(new SQLiteCommand(commandString), converter);

        /// <summary>
        /// Executes the provided SQLiteCommand using a new connection, returning a scalar value using the specified converter.
        /// </summary>
        /// <typeparam name="T">Type to return the scalar as</typeparam>
        /// <param name="command">Command text to execute</param>
        /// <param name="converter">Conversion method for returned value</param>
        /// <returns>An instance of T created by converting the returned value with the specified converter.</returns>
        protected T ExecuteScalar<T>(SQLiteCommand command, Func<object, T> converter = null)
        {
            using var connection = GetOpenConnection();
            command.Connection = connection;
            var scalar = command.ExecuteScalar();
            return converter != null ? converter(scalar) : (T)scalar;
        }

        /// <summary>
        /// Creates a SQLiteCommand from the provided string and executes it using a new connection, returning an IEnumerable of T generated from each row in the result set using the specified converter.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="commandString">Command text to execute</param>
        /// <param name="converter">Conversion method for rows</param>
        /// <returns>An IEnumerable of T created by converting the returned rows to T using the specified converter.</returns>
        protected IEnumerable<T> Execute<T>(string commandString, Func<SQLiteDataReader, T> converter) => Execute(new SQLiteCommand(commandString), converter);

        /// <summary>
        /// Executes the provided SQLiteCommand using a new connection, returning an IEnumerable of T generated from each row in the result set using the specified converter.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="command">Command to execute</param>
        /// <param name="converter">Conversion method for rows</param>
        /// <returns>An IEnumerable of T created by converting the returned rows to T using the specified converter.</returns>
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

        /// <summary>
        /// Wraps an action in a transaction which is committed on success, or rolled back on error - note that no additional connections should be opened within this to prevent deadlocks.
        /// </summary>
        /// <param name="action">Action to run in transaction</param>
        /// <param name="isolationLevel">Isolation Level of the transaction, defaults to Serializable</param>
        protected void ExecuteInTransaction(Action<SQLiteConnection> action, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction(isolationLevel);
            try
            {
                action(connection);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Retrieves the value from the named column, and converts it to an instance of T by deserializing the string value of the column.
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <param name="columnName">Name of the column</param>
        /// <returns>Value from named column as type T</returns>
        protected Func<SQLiteDataReader, T> DeserializeColumn<T>(string columnName) => GetColumnValue(columnName, c => c.ToString().Deserialize<T>());

        /// <summary>
        /// Retrieves the value from the named column, and converts it to an instance of T using the specified function.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="columnName">Name of the column</param>
        /// <param name="converter">Conversion function</param>
        /// <returns>Value from named column as type T</returns>
        protected Func<SQLiteDataReader, T> GetColumnValue<T>(string columnName, Func<object, T> converter) => reader => converter(reader[columnName]);
    }
}