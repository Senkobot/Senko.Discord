using System;
using System.Threading.Tasks;
using Senko.Discord.Gateway;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordGateway
	{
        ValueTask RestartAsync();

        ValueTask SendAsync(int shardId, GatewayOpcode opCode, object payload);

		ValueTask StartAsync();

		ValueTask StopAsync();
	}
}