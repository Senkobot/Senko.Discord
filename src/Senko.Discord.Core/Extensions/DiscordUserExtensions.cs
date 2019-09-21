using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Senko.Discord
{
    public static class DiscordUserExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDisplayName(this IDiscordUser user)
        {
            return (user as IDiscordGuildUser)?.Nickname ?? user.Username;
        }
    }
}
