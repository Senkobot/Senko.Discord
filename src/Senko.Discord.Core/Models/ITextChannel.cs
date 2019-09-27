using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordTextChannel : IDiscordChannel
	{
		ValueTask DeleteMessagesAsync(params ulong[] id);

		ValueTask DeleteMessagesAsync(params IDiscordMessage[] id);

		ValueTask<IEnumerable<IDiscordMessage>> GetMessagesAsync(int amount = 100);

		ValueTask<IDiscordMessage> GetMessageAsync(ulong id);

		ValueTask<IDiscordMessage> SendMessageAsync(string content, bool isTTS = false, DiscordEmbed embed = null);

		ValueTask<IDiscordMessage> SendFileAsync(Stream file, string fileName, string content = null, bool isTTs = false, DiscordEmbed embed = null);

		ValueTask TriggerTypingAsync();
	}

	public enum GetMessageType
	{
		Around, Before, After
	}
}