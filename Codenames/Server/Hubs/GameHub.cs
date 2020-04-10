using Codenames.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Codenames.Server.Hubs
{
    public class GameHub : Hub
    {
        public async Task UpdateGameAsync(string gameId, string updatedGame) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);

        public async Task UpdatePlayerIdentificationAsync(string gameId) => await Clients.OthersInGroup(gameId).SendAsync("UpdatePlayerIdentification");

        public async Task AddToGroupAsync(string chatId) => await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

        public async Task SendGlobalMessageAsync(string chatId, ChatMessage chatMessage) => await Clients.OthersInGroup(chatId).SendAsync("NewGlobalMessage", chatMessage);

        public async Task SendRoleMessageAsync(string chatId, ChatMessage chatMessage) => await Clients.OthersInGroup(chatId).SendAsync("NewRoleMessage", chatMessage);
    }
}