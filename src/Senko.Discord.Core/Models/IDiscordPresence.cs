using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordPresence
	{
		DiscordActivity Activity { get; }
		UserStatus Status { get; }
	}
}