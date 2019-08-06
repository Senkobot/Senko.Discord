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
        public static IServiceCollection AddDiscord(this IServiceCollection services, Action<DiscordOptions> configureOptions)
        {
            AddDiscord(services);
            services.Configure(configureOptions);
            return services;
        }

        public static IServiceCollection AddDiscord(this IServiceCollection services)
        {
            services.AddSingleton<IDiscordClient, DiscordClient>();

            services.AddSingleton<IDiscordApiClient, DiscordApiClient>();
            services.AddSingleton<IDiscordApiRateLimiter, DiscordApiRateLimiter>();

            services.AddSingleton<IDiscordGateway, GatewayCluster>();
            services.AddSingleton<IDiscordPacketHandler, DiscordPacketHandler>();
            services.AddTransient<IDiscordConnectionRatelimiter, DiscordConnectionRatelimiter>();

            return services;
        }
    }
}
