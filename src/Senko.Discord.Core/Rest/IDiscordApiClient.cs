using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Senko.Discord.Gateway;
using Senko.Discord.Packets;

namespace Senko.Discord.Rest
{
	public interface IDiscordApiClient : IGatewayApiClient
	{
		ValueTask AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays = 7, string reason = null);

		ValueTask AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId);

		ValueTask<DiscordChannelPacket> CreateDMChannelAsync(ulong userId);

		ValueTask<DiscordEmoji> CreateEmojiAsync(ulong guildId, EmojiCreationArgs args);

		ValueTask CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

		ValueTask<DiscordRolePacket> CreateGuildRoleAsync(ulong guildId, CreateRoleArgs args);

		ValueTask DeleteChannelAsync(ulong channelId);

		ValueTask DeleteGuildAsync(ulong guildId);

		ValueTask DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

		ValueTask DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId);

		ValueTask DeleteReactionsAsync(ulong channelId, ulong messageId);

		ValueTask DeleteMessageAsync(ulong channelId, ulong messageId);

		ValueTask DeleteMessagesAsync(ulong channelId, ulong[] messages);

		ValueTask<DiscordMessagePacket> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs args);

		ValueTask<DiscordRolePacket> EditRoleAsync(ulong guildId, DiscordRolePacket role);

		ValueTask<DiscordUserPacket> GetCurrentUserAsync();

		ValueTask<DiscordChannelPacket> GetChannelAsync(ulong channelId);

		ValueTask<DiscordChannelPacket[]> GetChannelsAsync(ulong guildId);

        ValueTask<DiscordChannelPacket[]> GetDMChannelsAsync();

        ValueTask<DiscordGuildPacket> GetGuildAsync(ulong guildId);

        ValueTask<DiscordGuildMemberPacket[]> GetGuildMembersAsync(ulong guildId);

        ValueTask<DiscordGuildMemberPacket> GetGuildUserAsync(ulong userId, ulong guildId);

		ValueTask<DiscordMessagePacket> GetMessageAsync(ulong channelId, ulong messageId);

        IAsyncEnumerable<DiscordMessagePacket> GetMessagesAsync(ulong channelId, int amount = 100);

        ValueTask<int> GetPruneCountAsync(ulong guildId, int days);

		ValueTask<DiscordUserPacket[]> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emojiId);

		ValueTask<DiscordRolePacket> GetRoleAsync(ulong roleId, ulong guildId);

		ValueTask<DiscordRolePacket[]> GetRolesAsync(ulong guildId);

		ValueTask<DiscordUserPacket> GetUserAsync(ulong userId);

		ValueTask ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberArgs packet);

        ValueTask ModifySelfAsync(UserModifyArgs args);

        ValueTask<int?> PruneGuildMembersAsync(ulong guildId, int days, bool computePrunedCount);

		ValueTask RemoveGuildBanAsync(ulong guildId, ulong userId);

		ValueTask KickGuildMemberAsync(ulong guildId, ulong userId, string reason = null);

		ValueTask RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId);

		ValueTask<DiscordMessagePacket> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs args);

		ValueTask<DiscordMessagePacket> SendMessageAsync(ulong channelId, MessageArgs args);

		ValueTask TriggerTypingAsync(ulong channelId);
	}
}