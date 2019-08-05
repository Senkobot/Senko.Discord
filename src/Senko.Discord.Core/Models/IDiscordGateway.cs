using System;
using System.Threading.Tasks;
using Senko.Discord.Gateway;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordGateway
	{
        #region Events

        event Func<DiscordChannelPacket, Task> OnChannelCreate;
        event Func<DiscordChannelPacket, Task> OnChannelUpdate;
        event Func<DiscordChannelPacket, Task> OnChannelDelete;

        event Func<DiscordGuildPacket, Task> OnGuildCreate;
        event Func<DiscordGuildPacket, Task> OnGuildUpdate;
        event Func<DiscordGuildUnavailablePacket, Task> OnGuildDelete;

        event Func<DiscordGuildMemberPacket, Task> OnGuildMemberAdd;
        event Func<ulong, DiscordUserPacket, Task> OnGuildMemberRemove;
        event Func<GuildMemberUpdateEventArgs, Task> OnGuildMemberUpdate;

        event Func<ulong, DiscordUserPacket, Task> OnGuildBanAdd;
        event Func<ulong, DiscordUserPacket, Task> OnGuildBanRemove;

        event Func<ulong, DiscordEmoji[], Task> OnGuildEmojiUpdate;

        event Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate;
        event Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate;
        event Func<ulong, ulong, Task> OnGuildRoleDelete;

        event Func<DiscordMessagePacket, Task> OnMessageCreate;
        event Func<DiscordMessagePacket, Task> OnMessageUpdate;
        event Func<MessageDeleteArgs, Task> OnMessageDelete;
        event Func<MessageBulkDeleteEventArgs, Task> OnMessageDeleteBulk;

        event Func<DiscordPresencePacket, Task> OnPresenceUpdate;

        event Func<GatewayReadyPacket, Task> OnReady;
        event Func<GatewayReadyPacket, Task> OnResume;

        event Func<TypingStartEventArgs, Task> OnTypingStart;

        event Func<DiscordUserPacket, Task> OnUserUpdate;
        #endregion

        Task RestartAsync();

        Task SendAsync(int shardId, GatewayOpcode opCode, object payload);

		Task StartAsync();

		Task StopAsync();
	}
}