using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Cryptonyms.Client.Services.SignalR
{
    public interface IHubCommunicator
    {
        void RegisterHandler<T>(string name, Action<T> handler);

        void RegisterHandler<S, T>(string name, Action<S, T> handler);

        Task StartAsync();
    }

    public abstract class HubCommunicator : IHubCommunicator, IAsyncDisposable
    {
        protected readonly HubConnection _hubConnection;

        protected HubCommunicator(string relativeUri, NavigationManager navigationManager)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(navigationManager.ToAbsoluteUri(relativeUri)).WithAutomaticReconnect().Build();
        }

        public void RegisterHandler(string name, Action handler) => _hubConnection.On(name, handler);

        public void RegisterHandler<T>(string name, Action<T> handler) => _hubConnection.On(name, handler);

        public void RegisterHandler<S, T>(string name, Action<S, T> handler) => _hubConnection.On(name, handler);

        public Task StartAsync() => _hubConnection.StartAsync();

        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return _hubConnection.DisposeAsync();
        }
    }
}
