using Cryptonyms.Client.Services;
using Cryptonyms.Client.Services.SignalR;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Threading.Tasks;

namespace Cryptonyms.Client
{
    internal static class Program
    {
        internal static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpService(builder.HostEnvironment.BaseAddress);
            builder.Services.AddGameStorage();
            builder.Services.AddBlazorTimer();
            builder.Services.AddHubCommunicator<GameHubCommunicator>();

            await builder.Build().RunAsync();
        }
    }
}