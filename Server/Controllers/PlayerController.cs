using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Repository;
using Cryptonyms.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
        public void New(JsonElement json) => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.AddPlayer(deviceId, player), json.GetStringProperty("DeviceId"), json.GetObjectProperty<Player>("Player"));

        [HttpPost("Update")]
        public void Update(JsonElement json) => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.UpdatePlayer(deviceId, player), json.GetStringProperty("DeviceId"), json.GetObjectProperty<Player>("Player"));

        [HttpGet("Get")]
        public string Get(string deviceId, string name) => UpdateDeviceLastSeenAndExecute((deviceId, name) => _playerRepository.GetPlayer(deviceId, name).Serialize(), deviceId, name);

        [HttpGet("List")]
        public string List(string deviceId) => UpdateDeviceLastSeenAndExecute(deviceId => _playerRepository.GetPlayers(deviceId).Serialize(), deviceId);

        [HttpDelete("Delete")]
        public void Delete(string deviceId, string name) => UpdateDeviceLastSeenAndExecute((deviceId, player) => _playerRepository.DeletePlayer(deviceId, name), deviceId, name);

        [HttpPost("RandomiseTeams")]
        public string RandomiseTeams(string deviceId)
        {
            return UpdateDeviceLastSeenAndExecute(deviceId =>
            {
                var currentPlayers = _playerRepository.GetPlayers(deviceId).ToList();
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

                _playerRepository.ReplacePlayers(deviceId, modifiedPlayers);

                return modifiedPlayers.Serialize();
            }, deviceId);
        }

        private void UpdateDeviceLastSeenAndExecute<T>(Action<string, T> action, string deviceId, T input)
        {
            _deviceRepository.AddOrUpdateDevice(deviceId);
            action(deviceId, input);
        }

        private TReturn UpdateDeviceLastSeenAndExecute<TReturn>(Func<string, TReturn> func, string deviceId)
        {
            _deviceRepository.AddOrUpdateDevice(deviceId);
            return func(deviceId);
        }

        private TReturn UpdateDeviceLastSeenAndExecute<T, TReturn>(Func<string, T, TReturn> func, string deviceId, T input)
        {
            _deviceRepository.AddOrUpdateDevice(deviceId);
            return func(deviceId, input);
        }
    }
}