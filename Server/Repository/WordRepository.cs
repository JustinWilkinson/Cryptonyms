using Cryptonyms.Server.Configuration;
using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.FileReaders;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Cryptonyms.Server.Repository
{
    public interface IWordRepository
    {
        void CreateWord(string word);

        IEnumerable<EditableWord> ListWords();

        int GetCount();

        void DeleteWord(string word);
    }

    public class WordRepository : Repository, IWordRepository
    {
        private readonly ILogger<WordRepository> _logger;

        public WordRepository(ILogger<WordRepository> logger, IFileReader fileReader, IOptions<ApplicationOptions> options) : base("CREATE TABLE IF NOT EXISTS Words (Word text PRIMARY KEY, IsSeed integer NOT NULL CHECK (IsSeed IN (0,1)))")
        {
            _logger = logger;

            try
            {
                if (ExecuteScalar("SELECT COUNT(*) AS WordCount FROM Words", Convert.ToInt32) == 0)
                {
                    ExecuteInTransaction((connection) => 
                    {
                        foreach (var word in fileReader.ReadFileLines(options.Value.SeedWordsPath))
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

        public void CreateWord(string word)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Words (Word, IsSeed) VALUES(@Word, 0)");
                command.AddParameter("@Word", word);
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred creating new word '{word}'.");
                throw;
            }
        }

        public int GetCount()
        {
            try
            {
                return Execute("SELECT COUNT(*) AS Count FROM Words", reader => Convert.ToInt32(reader["Count"])).Single();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving a count of all words.");
                throw;
            }
        }

        public IEnumerable<EditableWord> ListWords()
        {
            try
            {
                return Execute("SELECT * FROM Words", reader => new EditableWord { Text = reader["Word"].ToString(), Editable = Convert.ToInt32(reader["IsSeed"]) == 0 } );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred listing all words.");
                throw;
            }
        }

        public void DeleteWord(string word)
        {
            try
            {
                var command = new SQLiteCommand("DELETE FROM Words WHERE Word = @Word AND IsSeed = 0;");
                command.AddParameter("@Word", word);
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred deleting word '{word}'.");
                throw;
            }
        }
    }
}