using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private static readonly Random Random = new();
        private readonly ILogger<PlayersController> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IDeviceRepository _deviceRepository;

        public PlayersController(ILogger<PlayersController> logger, IPlayerRepository playerRepository, IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _playerRepository = playerRepository;
            _deviceRepository = deviceRepository;
        }

        [HttpPost("New")]
        public Task New(JsonElement json)
            => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.AddPlayer(deviceId, player), json.GetStringProperty("DeviceId"), json.GetObjectProperty<Player>("Player"));

        [HttpPut("Update")]
        public Task Update(JsonElement json)
            => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.UpdatePlayer(deviceId, player), json.GetStringProperty("DeviceId"), json.GetObjectProperty<Player>("Player"));

        [HttpGet("Get")]
        public Task<Player> Get(string deviceId, string name)
            => UpdateDeviceLastSeenAndExecute((deviceId, name) => _playerRepository.GetPlayer(deviceId, name), deviceId, name);

        [HttpGet("List")]
        public Task<IEnumerable<Player>> List(string deviceId)
            => UpdateDeviceLastSeenAndExecute(deviceId => _playerRepository.GetPlayers(deviceId).ToEnumerableAsync(), deviceId);

        [HttpDelete("Delete")]
        public Task Delete(string deviceId, string name)
            => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.DeletePlayer(deviceId, name), deviceId, name);

        [HttpPost("RandomiseTeams")]
        public Task<IEnumerable<Player>> RandomiseTeams(string deviceId)
        {
            return UpdateDeviceLastSeenAndExecute(async deviceId =>
            {
                var currentPlayers = (await _playerRepository.GetPlayers(deviceId).ToEnumerableAsync()).ToList();
                var minPlayersPerTeam = (int)Math.Round(currentPlayers.Count / 2d);
                var random = new Random();

                var modifiedPlayers = new List<Player>();
                foreach (var player in currentPlayers)
                {
                    var modifiedPlayer = player with
                    {
                        Team = (Team)random.Next(0, 2),
                        IsSpymaster = false
                    };

                    if (modifiedPlayers.Count(x => x.Team == modifiedPlayer.Team) >= minPlayersPerTeam)
                    {
                        modifiedPlayer.Team = modifiedPlayer.Team.Value.OpposingTeam();
                    }

                    modifiedPlayers.Add(modifiedPlayer);
                }

                var redTeam = modifiedPlayers.Where(x => x.Team == Team.Red).ToArray();
                var blueTeam = modifiedPlayers.Where(x => x.Team == Team.Blue).ToArray();

                if (redTeam.Length > 0)
                {
                    redTeam[Random.Next(0, redTeam.Length)].IsSpymaster = true;
                }

                if (blueTeam.Length > 0)
                {
                    blueTeam[Random.Next(0, blueTeam.Length)].IsSpymaster = true;
                }

                await _playerRepository.ReplacePlayers(deviceId, modifiedPlayers);

                return modifiedPlayers.AsEnumerable();
            }, deviceId);
        }

        private async Task UpdateDeviceLastSeenAndExecute<T>(Action<string, T> action, string deviceId, T input)
        {
            await _deviceRepository.AddOrUpdateDeviceAsync(deviceId);
            action(deviceId, input);
        }

        private async Task<TReturn> UpdateDeviceLastSeenAndExecute<TReturn>(Func<string, Task<TReturn>> func, string deviceId)
        {
            await _deviceRepository.AddOrUpdateDeviceAsync(deviceId);
            return await func(deviceId);
        }

        private async Task<TReturn> UpdateDeviceLastSeenAndExecute<T, TReturn>(Func<string, T, Task<TReturn>> func, string deviceId, T input)
        {
            await _deviceRepository.AddOrUpdateDeviceAsync(deviceId);
            return await func(deviceId, input);
        }
    }
}