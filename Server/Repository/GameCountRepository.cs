using Cryptonyms.Server.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Interface for managing the GameCount table.
    /// </summary>
    public interface IGameCountRepository
    {
        Task<int> GetGameCountAsync();

        Task IncrementGameCountAsync();
    }

    /// <summary>
    /// Manages the GameCount table.
    /// </summary>
    public class GameCountRepository : Repository, IGameCountRepository
    {
        private readonly ILogger<GameCountRepository> _logger;

        protected override string CreateStatement { get; } = "CREATE TABLE IF NOT EXISTS GameCount (GameCount integer)";

        public GameCountRepository(ILogger<GameCountRepository> logger)
        {
            _logger = logger;
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await base.InitializeAsync();
                if (await ExecuteScalarAsync("SELECT COUNT(*) AS Count FROM GameCount", Convert.ToInt32) == 0)
                {
                    await ExecuteAsync("INSERT INTO GameCount VALUES (0)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred initialising the Game Count Repository");
                throw;
            }
        }

        public async Task<int> GetGameCountAsync()
        {
            try
            {
                return await ExecuteAsync("SELECT * FROM GameCount", GetColumnValue("GameCount", Convert.ToInt32)).SingleAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving the current game count.");
                throw;
            }
        }

        public async Task IncrementGameCountAsync()
        {
            try
            {
                await ExecuteAsync("UPDATE GameCount SET GameCount = GameCount + 1");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred incrementing the game count.");
                throw;
            }
        }
    }
}