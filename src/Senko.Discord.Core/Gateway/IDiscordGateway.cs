using System;
using System.Threading.Tasks;
using Senko.Discord.Gateway;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordGateway
	{
        Task RestartAsync();

        Task SendAsync(int shardId, GatewayOpcode opCode, object payload);

		Task StartAsync();

		Task StopAsync();
	}
}