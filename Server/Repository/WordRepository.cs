using Codenames.Server.Extensions;
using Codenames.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Codenames.Server.Repository
{
    public interface IWordRepository
    {
        void CreateWord(string word);

        void EditWord(string originalWord, string updatedWord);

        IEnumerable<string> ListWords();

        void DeleteWord(string word);
    }

    public class WordRepository : Repository, IWordRepository
    {
        private readonly ILogger<WordRepository> _logger;

        public WordRepository(ILogger<WordRepository> logger) : base("CREATE TABLE IF NOT EXISTS Words (Word text PRIMARY KEY)")
        {
            _logger = logger;

            try
            {
                if (ExecuteScalar("SELECT COUNT(*) AS WordCount FROM Words", Convert.ToInt32) == 0)
                {
                    ExecuteInTransaction((connection) => 
                    {
                        foreach (var word in Data.SeedWords)
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