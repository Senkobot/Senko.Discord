using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordChannel : ISnowflake
	{
        ulong? GuildId { get; }

		bool IsNsfw { get; }

		string Name { get; }

		Task DeleteAsync();
	}

	public interface IDiscordGuildChannel : IDiscordChannel
	{
		new ulong GuildId { get; }

		ChannelType Type { get; }

		Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

		Task<IDiscordGuildUser> GetUserAsync(ulong id);

		Task<IDiscordGuild> GetGuildAsync();
	}
}