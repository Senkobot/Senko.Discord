using Senko.Discord.Helpers;
using Senko.Discord.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	internal class DiscordTextChannel : DiscordChannel, IDiscordTextChannel
	{
		public DiscordTextChannel(DiscordChannelPacket packet, IDiscordClient client)
			: base(packet, client)
		{
		}
		public ValueTask DeleteMessagesAsync(params ulong[] messageIds)
		{
			return _client.DeleteMessagesAsync(Id, messageIds);
		}

		public async ValueTask DeleteMessagesAsync(params IDiscordMessage[] messages)
		{
			await DeleteMessagesAsync(messages.Select(x => x.Id).ToArray());
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
