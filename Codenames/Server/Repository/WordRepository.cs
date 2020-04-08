using Codenames.Server.Extensions;
using Codenames.Shared;
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
        public WordRepository() : base("CREATE TABLE IF NOT EXISTS Words (Word text)")
        {
            if (ExecuteScalar("SELECT COUNT(*) AS WordCount FROM Words", Convert.ToInt32) == 0)
            {
                using var connection = GetOpenConnection();
                var transaction = connection.BeginTransaction();
                foreach (var word in Data.SeedWords)
                {
                    var command = new SQLiteCommand("INSERT INTO Words (Word) VALUES(@Word)", connection);
                    command.AddParameter("@Word", word);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        public void CreateWord(string word)
        {
            var command = new SQLiteCommand("INSERT INTO Words (Word) VALUES(@Word)");
            command.AddParameter("@Word", word);
            Execute(command);
        }

        public void EditWord(string originalWord, string updatedWord)
        {
            var command = new SQLiteCommand("UPDATE Words SET Word = @UpdateWord WHERE Word = @OriginalWord");
            command.AddParameter("@OriginalWord", originalWord);
            command.AddParameter("@UpdateWord", updatedWord);
            Execute(command);
        }

        public IEnumerable<string> ListWords() => Execute("SELECT * FROM Words", reader => reader["Word"].ToString());

        public void DeleteWord(string word)
        {
            var command = new SQLiteCommand("DELETE FROM Words WHERE Word = @Word");
            command.AddParameter("@Word", word);
            Execute(command);
        }
    }
}