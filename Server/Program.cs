using Cryptonyms.Server.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Threading.Tasks;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Cryptonyms.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                var webHost = CreateHostBuilder(args).Build();

                var jobScheduler = await JobScheduler.GetSchedulerAsync();
                await jobScheduler.ScheduleDailyJobAsync<CleanUpJob>("CleanUp", "DailyCleanUpTrigger");

                webHost.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "A fatal exception occurred causing the application to terminate.");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStaticWebAssets();
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}