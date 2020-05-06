using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public class DiscordMessage : IDiscordMessage
	{
		private readonly DiscordMessagePacket _packet;
		private readonly IDiscordClient _client;

		public DiscordMessage(DiscordMessagePacket packet, IDiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

        public IReadOnlyList<IDiscordAttachment> Attachments
            => _packet.Attachments
                .Select(x => new DiscordAttachment(x))
                .ToList();

		public IDiscordUser Author
        {
            get
            {
                if (_packet.Member == null)
                {
                    return new DiscordUser(_packet.Author, _client);
                }
                else
                {
                    _packet.Member.User = _packet.Author;
                    return new DiscordGuildUser(_packet.Member, _client);
                }
            }
        }

		public string Content
			=> _packet.Content;

		public ulong ChannelId
			=> _packet.ChannelId;

        public ulong? GuildId
            => _packet.GuildId;

        public IReadOnlyList<ulong> MentionedUserIds
			=> _packet.Mentions.Select(x => x.Id)
                .ToList();

		public DateTimeOffset Timestamp
			=> _packet.Timestamp;

		public ulong Id
			=> _packet.Id;

		public DiscordMessageType Type
			=> _packet.Type;

		public ValueTask<IDiscordMessage> EditAsync(EditMessageArgs args)
        {
            return _client.EditMessageAsync(ChannelId, Id, args.Content, args.Embed);
        }

        public ValueTask DeleteAsync()
        {
            return _client.DeleteMessageAsync(_packet.ChannelId, _packet.Id);
        }

        public async ValueTask<IDiscordTextChannel> GetChannelAsync()
        {
            var channel = await _client.GetChannelAsync(_packet.ChannelId, _packet.GuildId);
            return channel as IDiscordTextChannel;
        }

		public ValueTask<IEnumerable<IDiscordUser>> GetReactionsAsync(DiscordEmoji emoji)
        {
            return _client.GetReactionsAsync(_packet.ChannelId, Id, emoji);
        }

        public ValueTask CreateReactionAsync(DiscordEmoji emoji)
        {
            return _client.CreateReactionAsync(ChannelId, Id, emoji);
        }

        public ValueTask DeleteReactionAsync(DiscordEmoji emoji)
        {
            return _client.DeleteReactionAsync(ChannelId, Id, emoji);
        }

        public ValueTask DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user)
        {
            return DeleteReactionAsync(emoji, user.Id);
        }

        public ValueTask DeleteReactionAsync(DiscordEmoji emoji, ulong userId)
        {
            return _client.DeleteReactionAsync(ChannelId, Id, emoji, userId);
        }

        public ValueTask DeleteAllReactionsAsync()
        {
            return _client.DeleteReactionsAsync(ChannelId, Id);
        }
    }
}