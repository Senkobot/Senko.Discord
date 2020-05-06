using Microsoft.Extensions.DependencyInjection;
using Senko.Discord.Gateway.Ratelimiting;

namespace Senko.Discord.Gateway
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDiscordGateway<TEventHandler>(this IServiceCollection services)
            where TEventHandler : class, IDiscordEventHandler
        {
            services.AddSingleton<IDiscordEventHandler, TEventHandler>();

            return AddDiscordGateway(services);
        }
        
        public static IServiceCollection AddDiscordGateway(this IServiceCollection services)
        {
            services.AddSingleton<IDiscordGateway, GatewayCluster>();
            services.AddTransient<IDiscordConnectionRatelimiter, DiscordConnectionRatelimiter>();

            return services;
        }
    }
}
