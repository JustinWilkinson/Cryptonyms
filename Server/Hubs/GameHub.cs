using Cryptonyms.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Hubs
{
    public class GameHub : Hub, IGameHub
    {
        public Task AddToGroupAsync(string chatId) => Groups.AddToGroupAsync(Context.ConnectionId, chatId);

        public Task RemoveFromGroupAsync(string chatId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);

        public Task UpdateGameAsync(string gameId, string updatedGame) => Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);

        public Task UpdatePlayerIdentificationAsync(string gameId, string playerName, bool identified) => Clients.OthersInGroup(gameId).SendAsync("UpdatePlayerIdentification", playerName, identified);

        public Task SendGameMessageAsync(string chatId, string eventName, GameMessage chatMessage) => Clients.OthersInGroup(chatId).SendAsync(eventName, chatMessage);

        public Task AddNewPlayerToGameAsync(string gameId, Player player) => Clients.OthersInGroup(gameId).SendAsync("AddNewPlayerToGame", player);

        public Task NewGameAddedAsync() => Clients.Group("GamesPage").SendAsync("NewGameAdded");
    }
}