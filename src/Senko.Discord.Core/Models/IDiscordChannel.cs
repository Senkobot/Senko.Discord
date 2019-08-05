using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordChannel : ISnowflake
	{
		bool IsNsfw { get; }

		string Name { get; }

		Task DeleteAsync();
	}

	public interface IDiscordGuildChannel : IDiscordChannel
	{
		ulong GuildId { get; }

		ChannelType Type { get; }

		Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user);

		Task<IDiscordGuildUser> GetUserAsync(ulong id);

		Task<IDiscordGuild> GetGuildAsync();
	}
}