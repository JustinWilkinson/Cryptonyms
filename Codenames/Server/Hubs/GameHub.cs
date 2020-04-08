using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Codenames.Server.Hubs
{
    public class GameHub : Hub
    {
        public async Task AddToGameGroupAsync(string gameId) => await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);

        public async Task UpdatePlayerIdentificationAsync(string gameId) => await Clients.OthersInGroup(gameId).SendAsync("UpdatePlayerIdentification");
    }
}