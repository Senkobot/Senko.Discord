using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord.Gateway
{
    public interface IDiscordPacketHandler
    {
        Task OnChannelCreate(DiscordChannelPacket packet);
        Task OnChannelUpdate(DiscordChannelPacket packet);
        Task OnChannelDelete(DiscordChannelPacket packet);

        Task OnGuildCreate(DiscordGuildPacket packet);
        Task OnGuildUpdate(DiscordGuildPacket packet);
        Task OnGuildDelete(DiscordGuildUnavailablePacket packet);

        Task OnGuildMemberAdd(DiscordGuildMemberPacket packet);
        Task OnGuildMemberRemove(GuildIdUserArgs packet);
        Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs packet);

        Task OnGuildBanAdd(GuildIdUserArgs packet);
        Task OnGuildBanRemove(GuildIdUserArgs packet);

        Task OnGuildEmojiUpdate(GuildEmojisUpdateEventArgs packet);

        Task OnGuildRoleCreate(RoleEventArgs packet);
        Task OnGuildRoleUpdate(RoleEventArgs packet);
        Task OnGuildRoleDelete(RoleDeleteEventArgs packet);

        Task OnMessageCreate(DiscordMessagePacket packet);
        Task OnMessageUpdate(DiscordMessagePacket packet);
        Task OnMessageDelete(MessageDeleteArgs packet);
        Task OnMessageDeleteBulk(MessageBulkDeleteEventArgs packet);

        Task OnPresenceUpdate(DiscordPresencePacket packet);

        Task OnReady(GatewayReadyPacket packet);
        Task OnResume(GatewayReadyPacket packet);

        Task OnTypingStart(TypingStartEventArgs packet);

        Task OnUserUpdate(DiscordUserPacket packet);
    }
}
