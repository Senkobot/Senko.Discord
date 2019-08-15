using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Discord
{
    public interface IDiscordEventHandler
    {
        Task OnGuildJoin(IDiscordGuild guild);

        Task OnGuildUpdate(IDiscordGuild guild);

        Task OnUserUpdate(IDiscordUser user);

        Task OnChannelCreate(IDiscordChannel channel);

        Task OnChannelUpdate(IDiscordChannel channel);

        Task OnChannelDelete(IDiscordChannel channel);

        Task OnGuildUnavailable(ulong guildId);

        Task OnGuildLeave(ulong guildId);

        Task OnGuildMemberDelete(IDiscordGuildUser member);

        Task OnGuildMemberUpdate(IDiscordGuildUser member);

        Task OnGuildMemberCreate(IDiscordGuildUser member);

        Task OnGuildRoleCreate(ulong guildId, IDiscordRole role);

        Task OnGuildRoleUpdate(ulong guildId, IDiscordRole role);

        Task OnGuildRoleDeleted(ulong guildId, ulong roleId);

        Task OnMessageCreate(IDiscordMessage message);

        Task OnMessageUpdate(IDiscordMessage message);

        Task OnMessageDeleted(ulong channelId, ulong messageId);

        Task OnGuildMemberRolesUpdate(IDiscordGuildUser member);
    }
}
