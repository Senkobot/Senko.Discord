using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public class DiscordGuildTextChannel : DiscordGuildChannel, IDiscordGuildChannel, IDiscordTextChannel
	{
		public DiscordGuildTextChannel(DiscordChannelPacket packet, IDiscordClient client, ulong guildId)
			: base(packet, client, guildId)
		{
		}

		public ValueTask DeleteMessagesAsync(params ulong[] messageIds)
		{
			if (messageIds.Length == 0)
            {
                return default;
            }

            if (messageIds.Length > 100)
            {
                throw new ArgumentException("Cannot delete more than 100 messages.", nameof(messageIds));
            }

			return _client.DeleteMessagesAsync(Id, messageIds);
		}

		public ValueTask DeleteMessagesAsync(params IDiscordMessage[] messages)
		{
			return DeleteMessagesAsync(messages.Select(x => x.Id).ToArray());
		}

		public ValueTask<IDiscordMessage> GetMessageAsync(ulong id)
		{
			return _client.GetMessageAsync(Id, id);
		}

		public IAsyncEnumerable<IDiscordMessage> GetMessagesAsync(int amount = 100)
		{
			return _client.GetMessagesAsync(Id, amount);
		}

		public ValueTask<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content, bool isTTS = false, DiscordEmbed embed = null)
        {
            return _client.SendFileAsync(
                Id,
                file,
                fileName,
                new MessageArgs(content, embed, isTTS));
        }

        public ValueTask<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null)
        {
            return _client.SendMessageAsync(Id, new MessageArgs(content, embed, isTTS));
        }

        public ValueTask TriggerTypingAsync()
		{
			return _client.TriggerTypingAsync(Id);
		}
	}
}