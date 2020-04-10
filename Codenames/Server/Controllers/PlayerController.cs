using Codenames.Server.Extensions;
using Codenames.Server.Repository;
using Codenames.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Codenames.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IPlayerRepository _playerRepository;

        public PlayerController(ILogger<PlayerController> logger, IPlayerRepository PlayerRepository)
        {
            _logger = logger;
            _playerRepository = PlayerRepository;
        }

        [HttpPut("New")]
        public void New(JsonElement json) => _playerRepository.AddPlayer(json.GetStringProperty("DeviceId"), json.DeserializeStringProperty<Player>("Player"));

        [HttpPost("Update")]
        public void Update(JsonElement json) => _playerRepository.UpdatePlayer(json.GetStringProperty("DeviceId"), json.DeserializeStringProperty<Player>("Player"));

        [HttpGet("Get")]
        public string Get(string deviceId, string name) => _playerRepository.GetPlayer(deviceId, name).Serialize();

        [HttpGet("List")]
        public string List(string deviceId) => _playerRepository.GetPlayers(deviceId).Serialize();

        [HttpDelete("Delete")]
        public void Delete(string deviceId, string name) => _playerRepository.DeletePlayer(deviceId, name);

        [HttpPost("RandomiseTeams")]
        public string RandomiseTeams(string deviceId)
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
        }
    }
}