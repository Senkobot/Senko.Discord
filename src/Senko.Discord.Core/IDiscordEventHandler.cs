using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Discord
{
    public interface IDiscordEventHandler
    {
        Task OnGuildJoin(IDiscordGuild guild);

        Task OnUserUpdate(IDiscordUser user);

        Task OnGuildUnavailable(ulong guildId);

        Task OnGuildLeave(ulong guildId);

        Task OnGuildMemberDelete(IDiscordGuildUser member);

        Task OnGuildMemberCreate(IDiscordUser member);

        Task OnMessageCreate(IDiscordMessage message);

        Task OnMessageUpdate(IDiscordMessage message);
    }
}
