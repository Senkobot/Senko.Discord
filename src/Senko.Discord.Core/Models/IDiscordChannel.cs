using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordChannel : ISnowflake
	{
        ulong? GuildId { get; }

		bool IsNsfw { get; }

		string Name { get; }

		ValueTask DeleteAsync();
	}

	public interface IDiscordGuildChannel : IDiscordChannel
	{
		new ulong GuildId { get; }

		ChannelType Type { get; }

        ValueTask<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

        ValueTask<IDiscordGuildUser> GetUserAsync(ulong id);

		ValueTask<IDiscordGuild> GetGuildAsync();
	}
}