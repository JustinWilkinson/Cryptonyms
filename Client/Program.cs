using Cryptonyms.Client.Services;
using Cryptonyms.Client.Services.SignalR;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Cryptonyms.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddGameStorage();
            builder.Services.AddBlazorTimer();
            builder.Services.AddHubCommunicator<GameHubCommunicator>();

            await builder.Build().UseLocalTimeZone().RunAsync();
        }
    }
}