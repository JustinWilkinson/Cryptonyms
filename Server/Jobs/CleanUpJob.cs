using Cryptonyms.Server.Repository;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Jobs
{
    public class CleanUpJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var gameRepository = new GameRepository(new LoggerFactory().CreateLogger<GameRepository>());
            var messageRepository = new MessageRepository(new LoggerFactory().CreateLogger<MessageRepository>());
            var gameIdsToDelete = gameRepository.ListGames(true).Where(x => x.CompletedAtUtc.HasValue || x.StartedAtUtc < DateTime.UtcNow.AddDays(-5)).Select(x => x.GameId);
            gameRepository.DeleteGames(gameIdsToDelete);
            messageRepository.DeleteMessagesForGames(gameIdsToDelete);
            return Task.CompletedTask;
        }
    }
}