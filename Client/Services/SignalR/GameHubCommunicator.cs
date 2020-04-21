using Codenames.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Codenames.Client.Services.SignalR
{
    public class GameHubCommunicator : HubCommunicator, IGameHub
    {
        public GameHubCommunicator(NavigationManager navigationManager) : base("/GameHub", navigationManager)
        {

        }

        public async Task AddToGroupAsync(string groupId) => await _hubConnection.InvokeAsync("AddToGroupAsync", groupId);

        public async Task RemoveFromGroupAsync(string groupId) => await _hubConnection.InvokeAsync("RemoveFromGroupAsync", groupId);

        public async Task SendGameMessageAsync(string chatId, string eventName, GameMessage chatMessage) => await _hubConnection.InvokeAsync("SendGameMessageAsync", chatId, eventName, chatMessage);

        public async Task UpdateGameAsync(string gameId, string updatedGame) => await _hubConnection.InvokeAsync("UpdateGameAsync", gameId, updatedGame);

        public async Task UpdatePlayerIdentificationAsync(string gameId, string playerName, bool identified) => await _hubConnection.InvokeAsync("UpdatePlayerIdentificationAsync", gameId, playerName, identified);
    }
}