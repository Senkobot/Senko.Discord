﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord.Internal
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

		public async ValueTask<IDiscordMessage> EditAsync(EditMessageArgs args)
			=> await _client.EditMessageAsync(ChannelId, Id, args.Content, args.Embed);

		public async ValueTask DeleteAsync()
			=> await _client.ApiClient.DeleteMessageAsync(_packet.ChannelId, _packet.Id);

        public async ValueTask<IDiscordTextChannel> GetChannelAsync()
        {
            var channel = await _client.GetChannelAsync(_packet.ChannelId, _packet.GuildId);
            return channel as IDiscordTextChannel;
        }

		public async ValueTask<IEnumerable<IDiscordUser>> GetReactionsAsync(DiscordEmoji emoji)
			=> await _client.GetReactionsAsync(_packet.ChannelId, Id, emoji);

		public async ValueTask CreateReactionAsync(DiscordEmoji emoji)
			=> await _client.ApiClient.CreateReactionAsync(ChannelId, Id, emoji);

		public async ValueTask DeleteReactionAsync(DiscordEmoji emoji)
			=> await _client.ApiClient.DeleteReactionAsync(ChannelId, Id, emoji);

		public async ValueTask DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user)
			=> await DeleteReactionAsync(emoji, user.Id);

		public async ValueTask DeleteReactionAsync(DiscordEmoji emoji, ulong userId)
			=> await _client.ApiClient.DeleteReactionAsync(ChannelId, Id, emoji, userId);

		public async ValueTask DeleteAllReactionsAsync()
			=> await _client.ApiClient.DeleteReactionsAsync(ChannelId, Id);
	}
}