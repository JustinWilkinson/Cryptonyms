using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Codenames.Client.Services;
using Codenames.Client.Services.SignalR;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Codenames.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddStorage();
            builder.Services.AddBlazorTimer();
            builder.Services.AddHubCommunicator<GameHubCommunicator>();

            await builder.Build().UseLocalTimeZone().RunAsync();
        }
    }
}