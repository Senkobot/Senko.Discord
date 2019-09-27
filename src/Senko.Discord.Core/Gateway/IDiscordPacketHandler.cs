using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord.Gateway
{
    public interface IDiscordPacketHandler
    {
        ValueTask OnChannelCreate(DiscordChannelPacket packet);
        ValueTask OnChannelUpdate(DiscordChannelPacket packet);
        ValueTask OnChannelDelete(DiscordChannelPacket packet);

        ValueTask OnGuildCreate(DiscordGuildPacket packet);
        ValueTask OnGuildUpdate(DiscordGuildPacket packet);
        ValueTask OnGuildDelete(DiscordGuildUnavailablePacket packet);

        ValueTask OnGuildMemberAdd(DiscordGuildMemberPacket packet);
        ValueTask OnGuildMemberRemove(GuildIdUserArgs packet);
        ValueTask OnGuildMemberUpdate(GuildMemberUpdateEventArgs packet);

        ValueTask OnGuildBanAdd(GuildIdUserArgs packet);
        ValueTask OnGuildBanRemove(GuildIdUserArgs packet);

        ValueTask OnGuildEmojiUpdate(GuildEmojisUpdateEventArgs packet);

        ValueTask OnGuildRoleCreate(RoleEventArgs packet);
        ValueTask OnGuildRoleUpdate(RoleEventArgs packet);
        ValueTask OnGuildRoleDelete(RoleDeleteEventArgs packet);

        ValueTask OnMessageCreate(DiscordMessagePacket packet);
        ValueTask OnMessageUpdate(DiscordMessagePacket packet);
        ValueTask OnMessageDelete(MessageDeleteArgs packet);
        ValueTask OnMessageDeleteBulk(MessageBulkDeleteEventArgs packet);

        ValueTask OnMessageReactionAdd(MessageReactionArgs packet);
        ValueTask OnMessageReactionRemove(MessageReactionArgs packet);

        ValueTask OnPresenceUpdate(DiscordPresencePacket packet);

        ValueTask OnReady(GatewayReadyPacket packet);
        ValueTask OnResume(GatewayReadyPacket packet);

        ValueTask OnTypingStart(TypingStartEventArgs packet);

        ValueTask OnUserUpdate(DiscordUserPacket packet);
    }
}
