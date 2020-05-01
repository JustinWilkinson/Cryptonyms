using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Interface for managing the GameCount table.
    /// </summary>
    public interface IGameCountRepository
    {
        int GetGameCount();

        void IncrementGameCount();
    }

    /// <summary>
    /// Manages the GameCount table.
    /// </summary>
    public class GameCountRepository : Repository, IGameCountRepository
    {
        private readonly ILogger<GameCountRepository> _logger;

        public GameCountRepository(ILogger<GameCountRepository> logger) : base("CREATE TABLE IF NOT EXISTS GameCount (GameCount integer)")
        {
            _logger = logger;
            try
            {
                if (ExecuteScalar("SELECT COUNT(*) AS Count FROM GameCount", Convert.ToInt32) == 0)
                {
                    Execute("INSERT INTO GameCount VALUES (0)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred initialising the Game Count Repository");
                throw;
            }
        }

        public int GetGameCount()
        {
            try
            {
                return Execute("SELECT * FROM GameCount", GetColumnValue("GameCount", Convert.ToInt32)).Single();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving the current game count.");
                throw;
            }
        }

        public void IncrementGameCount()
        {
            try
            {
                Execute("UPDATE GameCount SET GameCount = GameCount + 1");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred incrementing the game count.");
                throw;
            }
        }
    }
}