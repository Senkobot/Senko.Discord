using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Senko.Discord.Example
{
    internal class Program
    {
        /// <summary>
        /// The Bot token.
        /// </summary>
        private const string Token = "";

        public static async Task Main(string[] args)
        {
            var provider = CreateServiceProvider();
            var client = provider.GetService<IDiscordClient>();

            await client.StartAsync();
            await WaitForCloseAsync();
            await client.StopAsync();
        }

        /// <summary>
        /// Create the service provider.
        /// </summary>
        /// <returns>The service provider.</returns>
        public static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ICacheClient, InMemoryCacheClient>();

            services.AddDiscord<DiscordEventHandler>(options =>
            {
                options.Token = Token;
            });

            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Wait for the host to press CTRL + C.
        /// </summary>
        private static async Task WaitForCloseAsync()
        {
            var semaphore = new SemaphoreSlim(0, 1);

            void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
            {
                semaphore.Release();
            }

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            await semaphore.WaitAsync();
            Console.CancelKeyPress -= ConsoleOnCancelKeyPress;
        }
    }
}
