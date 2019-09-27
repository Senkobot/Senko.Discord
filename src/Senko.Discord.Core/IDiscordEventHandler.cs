using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public interface IDiscordEventHandler
    {
        ValueTask OnGuildJoin(IDiscordGuild guild);

        ValueTask OnGuildUpdate(IDiscordGuild guild);

        ValueTask OnUserUpdate(IDiscordUser user);

        ValueTask OnChannelCreate(IDiscordChannel channel);

        ValueTask OnChannelUpdate(IDiscordChannel channel);

        ValueTask OnChannelDelete(IDiscordChannel channel);

        ValueTask OnGuildUnavailable(ulong guildId);

        ValueTask OnGuildLeave(ulong guildId);

        ValueTask OnGuildMemberDelete(IDiscordGuildUser member);

        ValueTask OnGuildMemberUpdate(IDiscordGuildUser member);

        ValueTask OnGuildMemberCreate(IDiscordGuildUser member);

        ValueTask OnGuildRoleCreate(ulong guildId, IDiscordRole role);

        ValueTask OnGuildRoleUpdate(ulong guildId, IDiscordRole role);

        ValueTask OnGuildRoleDeleted(ulong guildId, IDiscordRole role);

        ValueTask OnMessageCreate(IDiscordMessage message);

        ValueTask OnMessageUpdate(IDiscordMessage message);

        ValueTask OnMessageDeleted(ulong channelId, ulong messageId);

        ValueTask OnMessageEmojiCreated(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji);

        ValueTask OnMessageEmojiDeleted(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji);

        ValueTask OnGuildMemberRolesUpdate(IDiscordGuildUser member);
    }
}
