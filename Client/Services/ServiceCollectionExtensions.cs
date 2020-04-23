﻿using Cryptonyms.Client.Services.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptonyms.Client.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBlazorTimer(this IServiceCollection services) => services.AddTransient<BlazorTimer>();

        public static void AddHubCommunicator<T>(this IServiceCollection services) where T : HubCommunicator => services.AddTransient<T>();
    }
}