using System;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord.Gateway;
using Senko.Discord.Gateway.Ratelimiting;
using Senko.Discord.Http;
using Senko.Discord.Rest;
using Senko.Discord.Rest.Http;

namespace Senko.Discord
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDiscord<THandler>(this IServiceCollection services, Action<DiscordOptions> configureOptions)
            where THandler : class, IDiscordEventHandler
        {
            AddDiscord<THandler>(services);
            services.Configure(configureOptions);
            return services;
        }

        public static IServiceCollection AddDiscord<THandler>(this IServiceCollection services)
            where THandler : class, IDiscordEventHandler
        {
            services.AddSingleton<IDiscordEventHandler, THandler>();
            services.AddSingleton<IDiscordClient, DiscordClient>();

            services.AddSingleton<IDiscordApiClient, DiscordApiClient>();
            services.AddSingleton<IDiscordApiRateLimiter, DiscordApiRateLimiter>();

            services.AddSingleton<IDiscordGateway, GatewayCluster>();
            services.AddTransient<IDiscordConnectionRatelimiter, DiscordConnectionRatelimiter>();

            return services;
        }
    }
}
