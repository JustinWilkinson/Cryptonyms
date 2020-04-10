using Codenames.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Codenames.Server.Hubs
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string chatId);

        Task UpdateGameAsync(string gameId, string updatedGame);

        Task UpdatePlayerIdentificationAsync(string gameId);

        Task SendChatMessageAsync(string chatId, string eventName, ChatMessage chatMessage);
    }

    public class GameHub : Hub, IGameHub
    {
        public async Task AddToGroupAsync(string chatId) => await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame", updatedGame);

        public async Task UpdatePlayerIdentificationAsync(string gameId) => await Clients.OthersInGroup(gameId).SendAsync("UpdatePlayerIdentification");

        public async Task SendChatMessageAsync(string chatId, string eventName, ChatMessage chatMessage) => await Clients.OthersInGroup(chatId).SendAsync(eventName, chatMessage);
    }
}