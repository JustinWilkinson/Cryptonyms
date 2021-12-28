using Cryptonyms.Server.Extensions;
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
        private static readonly IGameRepository _gameRepository;
        private static readonly IMessageRepository _messageRepository;
        private static readonly IDeviceRepository _deviceRepository;
        private static readonly IPlayerRepository _playerRepository;
        private static readonly ILogger<CleanUpJob> _logger;

        static CleanUpJob()
        {
            var loggerFactory = new LoggerFactory();
            _gameRepository = new GameRepository(loggerFactory.CreateLogger<GameRepository>());
            _messageRepository = new MessageRepository(loggerFactory.CreateLogger<MessageRepository>());
            _deviceRepository = new DeviceRepository(loggerFactory.CreateLogger<DeviceRepository>());
            _playerRepository = new PlayerRepository(loggerFactory.CreateLogger<PlayerRepository>());
            _logger = loggerFactory.CreateLogger<CleanUpJob>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var gameIdsToDelete = await _gameRepository.ListGamesAsync(true)
                    .WhereAsync(x => x.CompletedAtUtc.HasValue || x.StartedAtUtc < DateTime.UtcNow.AddDays(-5))
                    .SelectAsync(x => x.GameId)
                    .ToEnumerableAsync();

                var devicesToDelete = await _deviceRepository.GetDevicesAsync()
                    .WhereAsync(x => x.LastSeenUtc < DateTime.UtcNow.AddDays(-30))
                    .SelectAsync(x => x.DeviceId)
                    .ToEnumerableAsync();

                await _gameRepository.DeleteGamesAsync(gameIdsToDelete);
                await _messageRepository.DeleteMessagesForGamesAsync(gameIdsToDelete);
                await _deviceRepository.DeleteDevicesAsync(devicesToDelete);
                await _playerRepository.DeletePlayersForDevices(devicesToDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred running a clean up job.");
            }
        }
    }
}