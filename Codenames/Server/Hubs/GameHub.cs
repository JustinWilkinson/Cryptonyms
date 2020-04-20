using Codenames.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Codenames.Server.Hubs
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string chatId);

        Task RemoveFromGroupAsync(string chatId);

        Task UpdateGameAsync(string gameId, string updatedGame);

        Task UpdatePlayerIdentificationAsync(string gameId, string playerName, bool identified);

        Task SendGameMessageAsync(string chatId, string eventName, GameMessage chatMessage);
    }

    public class GameHub : Hub, IGameHub
    {
        public async Task AddToGroupAsync(string chatId) => await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

        public async Task RemoveFromGroupAsync(string chatId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);

        public async Task UpdatePlayerIdentificationAsync(string gameId, string playerName, bool identified) => await Clients.OthersInGroup(gameId).SendAsync("UpdatePlayerIdentification", playerName, identified);

        public async Task SendGameMessageAsync(string chatId, string eventName, GameMessage chatMessage) => await Clients.OthersInGroup(chatId).SendAsync(eventName, chatMessage);


    }
}