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
		public async ValueTask DeleteMessagesAsync(params ulong[] id)
		{
			if (id.Length == 0)
			{
				throw new ArgumentNullException();
			}

			if (id.Length < 2)
			{
				await _client.ApiClient.DeleteMessageAsync(Id, id[0]);
			}

			if (id.Length > 100)
			{
				id = id.Take(100).ToArray();
			}

			await _client.ApiClient.DeleteMessagesAsync(Id, id);
		}

		public async ValueTask DeleteMessagesAsync(params IDiscordMessage[] messages)
		{
			await DeleteMessagesAsync(messages.Select(x => x.Id).ToArray());
		}

		public async ValueTask<IDiscordMessage> GetMessageAsync(ulong id)
		{
			return new DiscordMessage(await _client.ApiClient.GetMessageAsync(Id, id), _client);
		}

		public async ValueTask<IEnumerable<IDiscordMessage>> GetMessagesAsync(int amount = 100)
		{
			return (await _client.ApiClient.GetMessagesAsync(Id, amount))
				.Select(x => new DiscordMessage(x, _client));
		}

		public async ValueTask<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content, bool isTTS = false, DiscordEmbed embed = null)
			=> await _client.SendFileAsync(
                Id, 
                file, 
                fileName, 
                new MessageArgs(content, embed, isTTS));

        public async ValueTask<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null)
            => await DiscordChannelHelper.CreateMessageAsync(
                _client, 
                _packet, 
                new MessageArgs(content, embed, isTTS));

		public async ValueTask TriggerTypingAsync()
		{
			await _client.ApiClient.TriggerTypingAsync(Id);
		}
	}
}
