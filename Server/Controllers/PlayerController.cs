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
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IDeviceRepository _deviceRepository;

        public PlayerController(ILogger<PlayerController> logger, IPlayerRepository playerRepository, IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _playerRepository = playerRepository;
            _deviceRepository = deviceRepository;
        }

        [HttpPut("New")]
        public Task New(JsonElement json) => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.AddPlayerAsync(deviceId, player), json.GetStringProperty("DeviceId"), json.GetObjectProperty<Player>("Player"));

        [HttpPost("Update")]
        public Task Update(JsonElement json) => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.UpdatePlayerAsync(deviceId, player), json.GetStringProperty("DeviceId"), json.GetObjectProperty<Player>("Player"));

        [HttpGet("Get")]
        public Task<Player> Get(string deviceId, string name) => UpdateDeviceLastSeenAndExecute((deviceId, name) => _playerRepository.GetPlayerAsync(deviceId, name), deviceId, name);

        [HttpGet("List")]
        public Task<IEnumerable<Player>> List(string deviceId) => UpdateDeviceLastSeenAndExecute(deviceId => _playerRepository.GetPlayersAsync(deviceId).ToEnumerableAsync(), deviceId);

        [HttpDelete("Delete")]
        public Task Delete(string deviceId, string name) => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.DeletePlayerAsync(deviceId, name), deviceId, name);

        [HttpPost("RandomiseTeams")]
        public Task<IEnumerable<Player>> RandomiseTeams(string deviceId)
        {
            return UpdateDeviceLastSeenAndExecute(async deviceId =>
            {
                var currentPlayers = (await _playerRepository.GetPlayersAsync(deviceId).ToEnumerableAsync()).ToList();
                var minPlayersPerTeam = (int)Math.Round(currentPlayers.Count / 2d);
                var random = new Random();

                var modifiedPlayers = new List<Player>();
                foreach (var player in currentPlayers)
                {
                    var modifiedPlayer = new Player
                    {
                        Name = player.Name,
                        Team = (Team)random.Next(0, 2),
                        IsSpymaster = false,
                        Identified = player.Identified
                    };

                    if (modifiedPlayers.Count(x => x.Team == modifiedPlayer.Team) >= minPlayersPerTeam)
                    {
                        modifiedPlayer.Team = (Team)(1 - modifiedPlayer.Team);
                    }

                    modifiedPlayers.Add(modifiedPlayer);
                }

                var redTeam = modifiedPlayers.Where(x => x.Team == Team.Red).ToArray();
                var blueTeam = modifiedPlayers.Where(x => x.Team == Team.Blue).ToArray();

                if (redTeam.Length > 0)
                {
                    redTeam[random.Next(0, redTeam.Length)].IsSpymaster = true;
                }

                if (blueTeam.Length > 0)
                {
                    blueTeam[random.Next(0, blueTeam.Length)].IsSpymaster = true;
                }

                await _playerRepository.ReplacePlayersAsync(deviceId, modifiedPlayers);

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