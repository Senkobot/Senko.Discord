using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordMessage : ISnowflake
	{
        /// <summary>
        /// All attachments attached to this message.
        /// </summary>
        IReadOnlyList<IDiscordAttachment> Attachments { get; }

        /// <summary>
        /// The creator of the message.
        /// </summary>
        IDiscordUser Author { get; }

        string Content { get; }

        /// <summary>
        /// The channel this message was created in.
        /// </summary>
		ulong ChannelId { get; }

        /// <summary>
        /// The guild this message was created in.
        /// </summary>
        ulong? GuildId { get; }

        IReadOnlyList<ulong> MentionedUserIds { get; }

        DateTimeOffset Timestamp { get; }

        DiscordMessageType Type { get; }

        ValueTask CreateReactionAsync(DiscordEmoji emoji);

		ValueTask DeleteReactionAsync(DiscordEmoji emoji);

		ValueTask DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user);

		ValueTask DeleteReactionAsync(DiscordEmoji emoji, ulong userId);

		ValueTask DeleteAllReactionsAsync();

		ValueTask<IDiscordMessage> EditAsync(EditMessageArgs args);

        /// <summary>
        /// Deletes this message.
        /// </summary>
		ValueTask DeleteAsync();

		ValueTask<IDiscordTextChannel> GetChannelAsync();

		ValueTask<IEnumerable<IDiscordUser>> GetReactionsAsync(DiscordEmoji emoji);
	}
}