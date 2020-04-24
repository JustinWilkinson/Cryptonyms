using Cryptonyms.Server.Configuration;
using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.FileReaders;
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

        void EditWord(string originalWord, string updatedWord);

        IEnumerable<string> ListWords();

        int GetCount();

        void DeleteWord(string word);
    }

    public class WordRepository : Repository, IWordRepository
    {
        private readonly ILogger<WordRepository> _logger;

        public WordRepository(ILogger<WordRepository> logger, IFileReader fileReader, IOptions<ApplicationOptions> options) : base("CREATE TABLE IF NOT EXISTS Words (Word text PRIMARY KEY)")
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
                            var command = new SQLiteCommand("INSERT INTO Words (Word) VALUES(@Word)", connection);
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
                var command = new SQLiteCommand("INSERT INTO Words (Word) VALUES(@Word)");
                command.AddParameter("@Word", word);
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred creating new word '{word}'.");
                throw;
            }
        }

        public void EditWord(string originalWord, string updatedWord)
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Words SET Word = @UpdateWord WHERE Word = @OriginalWord");
                command.AddParameter("@OriginalWord", originalWord);
                command.AddParameter("@UpdateWord", updatedWord);
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred modifying word '{originalWord}' to '{updatedWord}'.");
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

        public IEnumerable<string> ListWords()
        {
            try
            {
                return Execute("SELECT * FROM Words", reader => reader["Word"].ToString());
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
                var command = new SQLiteCommand("DELETE FROM Words WHERE Word = @Word");
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