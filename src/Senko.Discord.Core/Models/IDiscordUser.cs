using System;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordUser : ISnowflake
	{
        DiscordUserPacket Packet { get; }

        string AvatarId { get; }

		string Mention { get; }

		string Username { get; }

		string NormalizedUsername { get; }

		string Discriminator { get; }

		DateTimeOffset CreatedAt { get; }

		bool IsBot { get; }

        ValueTask<IDiscordPresence> GetPresenceAsync();

        ValueTask<IDiscordTextChannel> GetDMChannelAsync();

		string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256);
	}
}