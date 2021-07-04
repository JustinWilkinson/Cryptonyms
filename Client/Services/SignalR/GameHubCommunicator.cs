using Cryptonyms.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Cryptonyms.Client.Services.SignalR
{
    public class GameHubCommunicator : HubCommunicator, IGameHub
    {
        public GameHubCommunicator(NavigationManager navigationManager) : base("/GameHub", navigationManager)
        {
        }

        public Task AddToGroupAsync(string groupId) => _hubConnection.InvokeAsync("AddToGroupAsync", groupId);

        public Task RemoveFromGroupAsync(string groupId) => _hubConnection.InvokeAsync("RemoveFromGroupAsync", groupId);

        public Task SendGameMessageAsync(string chatId, string eventName, GameMessage chatMessage) => _hubConnection.InvokeAsync("SendGameMessageAsync", chatId, eventName, chatMessage);

        public Task UpdateGameAsync(string gameId, string updatedGame) => _hubConnection.InvokeAsync("UpdateGameAsync", gameId, updatedGame);

        public Task UpdatePlayerIdentificationAsync(string gameId, string playerName, bool identified) => _hubConnection.InvokeAsync("UpdatePlayerIdentificationAsync", gameId, playerName, identified);

        public Task AddNewPlayerToGameAsync(string gameId, Player player) => _hubConnection.InvokeAsync("AddNewPlayerToGameAsync", gameId, player);

        public Task NewGameAddedAsync() => _hubConnection.InvokeAsync("NewGameAddedAsync");
    }
}