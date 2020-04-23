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
        private static readonly GameRepository _gameRepository;
        private static readonly MessageRepository _messageRepository;

        static CleanUpJob()
        {
            var loggerFactory = new LoggerFactory();
            _gameRepository = new GameRepository(loggerFactory.CreateLogger<GameRepository>());
            _messageRepository = new MessageRepository(loggerFactory.CreateLogger<MessageRepository>());
        }

        public Task Execute(IJobExecutionContext context)
        {
            var gameIdsToDelete = _gameRepository.ListGames(true).Where(x => x.CompletedAtUtc.HasValue || x.StartedAtUtc < DateTime.UtcNow.AddDays(-5)).Select(x => x.GameId);
            _gameRepository.DeleteGames(gameIdsToDelete);
            _messageRepository.DeleteMessagesForGames(gameIdsToDelete);
            return Task.CompletedTask;
        }
    }
}