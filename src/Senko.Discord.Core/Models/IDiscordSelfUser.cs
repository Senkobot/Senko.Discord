using System;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public interface IDiscordSelfUser : IDiscordUser
    {
        /// <summary>
        /// Gets recent DM channels for the current user. This function does not work on a BOT account.
        /// </summary>
        /// <remarks>Does not work on a BOT account.</remarks>
        ValueTask<IDiscordChannel> GetDMChannelsAsync();

        /// <summary>
        /// Modify the current user.
        /// </summary>
        /// <param name="modifyArgs"></param>
        ValueTask ModifyAsync(Action<UserModifyArgs> modifyArgs);
    }
}
