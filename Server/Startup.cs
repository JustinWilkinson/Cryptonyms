using AspNetCoreRateLimit;
using Cryptonyms.Server.Configuration;
using Cryptonyms.Server.Hubs;
using Cryptonyms.Server.Repository;
using Cryptonyms.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Database = Cryptonyms.Server.Repository.Repository;

namespace Cryptonyms.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            services.AddSignalR();

            // Seed database.
            var appSection = Configuration.GetSection("Application");
            Database.CreateDatabase(appSection.Get<ApplicationOptions>().ConnectionString);

            // Config options
            services.AddOptions();
            services.Configure<ApplicationOptions>(appSection);
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // Repositories
            services.AddSingleton<IGameCountRepository, GameCountRepository>();
            services.AddSingleton<IGameRepository, GameRepository>();
            services.AddSingleton<IPlayerRepository, PlayerRepository>();
            services.AddSingleton<IDeviceRepository, DeviceRepository>();
            services.AddSingleton<IWordRepository, WordRepository>();
            services.AddSingleton<IMessageRepository, MessageRepository>();

            // Rate limiter.
            services.AddMemoryCache();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Additional Services
            services.AddSingleton<IFileReader, FileReader>();
            services.AddSingleton<IProfanityFilter, ProfanityFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/GameHub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}