using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordReaction
	{
		/// <summary>
		/// The emoji information.
		/// </summary>
		DiscordEmoji Emoji { get; }

		/// <summary>
		/// The user information.
		/// </summary>
		IDiscordUser User { get; }
	}
}