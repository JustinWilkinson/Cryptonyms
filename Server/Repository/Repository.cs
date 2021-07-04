using Cryptonyms.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Base class for database interactions.
    /// </summary>
    public abstract class Repository
    {
        private static string _connectionString;
        private bool _initializing;
        private bool _initialized;

        protected abstract string CreateStatement { get; }

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
        /// Performs any necessary asynchronous initialization for the repository.
        /// </summary>
        protected virtual Task InitializeAsync() => ExecuteAsync(CreateStatement);

        /// <summary>
        /// Creates a SQLiteCommand from the provided string and executes it using a new connection.
        /// </summary>
        /// <param name="commandString">Command text to execute</param>
        protected Task ExecuteAsync(string commandString) => ExecuteAsync(new SQLiteCommand(commandString));

        /// <summary>
        /// Executes the provided command using a new connection.
        /// </summary>
        /// <param name="command">Command to execute</param>
        protected async Task ExecuteAsync(SQLiteCommand command)
        {
            using var connection = await GetOpenConnectionAsync();
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Creates a SQLiteCommand from the provided string and executes it using a new connection, returning a scalar value using the specified converter.
        /// </summary>
        /// <typeparam name="T">Type to return the scalar as</typeparam>
        /// <param name="commandString">Command text to execute</param>
        /// <param name="converter">Conversion method for returned value</param>
        /// <returns>An instance of T created by converting the returned value with the specified converter.</returns>
        protected Task<T> ExecuteScalarAsync<T>(string commandString, Func<object, T> converter = null) => ExecuteScalarAsync(new SQLiteCommand(commandString), converter);

        /// <summary>
        /// Executes the provided SQLiteCommand using a new connection, returning a scalar value using the specified converter.
        /// </summary>
        /// <typeparam name="T">Type to return the scalar as</typeparam>
        /// <param name="command">Command text to execute</param>
        /// <param name="converter">Conversion method for returned value</param>
        /// <returns>An instance of T created by converting the returned value with the specified converter.</returns>
        protected async Task<T> ExecuteScalarAsync<T>(SQLiteCommand command, Func<object, T> converter = null)
        {
            using var connection = await GetOpenConnectionAsync();
            command.Connection = connection;
            var scalar = await command.ExecuteScalarAsync();
            return converter is not null ? converter(scalar) : (T)scalar;
        }

        /// <summary>
        /// Creates a SQLiteCommand from the provided string and executes it using a new connection, returning an IEnumerable of T generated from each row in the result set using the specified converter.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="commandString">Command text to execute</param>
        /// <param name="converter">Conversion method for rows</param>
        /// <returns>An IEnumerable of T created by converting the returned rows to T using the specified converter.</returns>
        protected IAsyncEnumerable<T> ExecuteAsync<T>(string commandString, Func<SQLiteDataReader, T> converter) => ExecuteAsync(new SQLiteCommand(commandString), converter);

        /// <summary>
        /// Executes the provided SQLiteCommand using a new connection, returning an IEnumerable of T generated from each row in the result set using the specified converter.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="command">Command to execute</param>
        /// <param name="converter">Conversion method for rows</param>
        /// <returns>An IEnumerable of T created by converting the returned rows to T using the specified converter.</returns>
        protected async IAsyncEnumerable<T> ExecuteAsync<T>(SQLiteCommand command, Func<SQLiteDataReader, T> converter)
        {
            using var connection = await GetOpenConnectionAsync();
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
        protected async Task ExecuteInTransactionAsync(Action<SQLiteConnection> action, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            using var connection = await GetOpenConnectionAsync();
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

        private async Task<SQLiteConnection> GetOpenConnectionAsync()
        {
            await InitializeIfNecessaryAsync();
            var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private async ValueTask InitializeIfNecessaryAsync()
        {
            if (!_initialized && !_initializing)
            {
                _initializing = true;
                await InitializeAsync();
                _initialized = true;
            }
        }
    }
}