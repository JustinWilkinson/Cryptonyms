using Cryptonyms.Server.Configuration;
using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Services;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Interface for managing the Words table.
    /// </summary>
    public interface IWordRepository
    {
        Task CreateWordAsync(string word);

        IAsyncEnumerable<EditableWord> ListWordsAsync();

        Task<int> GetCountAsync();

        Task DeleteWordAsync(string word);
    }

    /// <summary>
    /// Manages the Words table.
    /// </summary>
    public class WordRepository : Repository, IWordRepository
    {
        private readonly ILogger<WordRepository> _logger;
        private readonly IFileReader _fileReader;
        private readonly IOptions<ApplicationOptions> _options;

        protected override string CreateStatement { get; } = "CREATE TABLE IF NOT EXISTS Words (Word text PRIMARY KEY, IsSeed integer NOT NULL CHECK (IsSeed IN (0,1)))";

        public WordRepository(ILogger<WordRepository> logger, IFileReader fileReader, IOptions<ApplicationOptions> options)
        {
            _logger = logger;
            _fileReader = fileReader;
            _options = options;
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await base.InitializeAsync();

                if (await ExecuteScalarAsync("SELECT COUNT(*) AS WordCount FROM Words", Convert.ToInt32) == 0)
                {
                    await ExecuteInTransactionAsync(async connection =>
                    {
                        await foreach (var word in _fileReader.ReadLinesAsync(_options.Value.SeedWordsPath))
                        {
                            var command = new SQLiteCommand("INSERT INTO Words (Word, IsSeed) VALUES(@Word, 1)", connection);
                            command.AddParameter("@Word", word);
                            command.ExecuteNonQuery();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred initialising the Word Repository");
                throw;
            }
        }

        public async Task CreateWordAsync(string word)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Words (Word, IsSeed) VALUES(@Word, 0)");
                command.AddParameter("@Word", word);
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred creating new word '{word}'.");
                throw;
            }
        }

        public async Task<int> GetCountAsync()
        {
            try
            {
                return await ExecuteAsync("SELECT COUNT(*) AS Count FROM Words", reader => Convert.ToInt32(reader["Count"])).SingleAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving a count of all words.");
                throw;
            }
        }

        public IAsyncEnumerable<EditableWord> ListWordsAsync()
        {
            try
            {
                return ExecuteAsync("SELECT * FROM Words", reader => new EditableWord { Text = reader["Word"].ToString(), Editable = Convert.ToInt32(reader["IsSeed"]) == 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred listing all words.");
                throw;
            }
        }

        public async Task DeleteWordAsync(string word)
        {
            try
            {
                var command = new SQLiteCommand("DELETE FROM Words WHERE Word = @Word AND IsSeed = 0;");
                command.AddParameter("@Word", word);
                await ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred deleting word '{word}'.");
                throw;
            }
        }
    }
}