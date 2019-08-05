using System;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord.Internal
{
    class DiscordSelfUser : DiscordUser, IDiscordSelfUser
    {
        public DiscordSelfUser(DiscordUserPacket user, IDiscordClient client)
            : base(user, client)
        { }

        public Task<IDiscordChannel> GetDMChannelsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task ModifyAsync(Action<UserModifyArgs> modifyArgs)
        {
            var args = new UserModifyArgs();
            modifyArgs(args);
            await Client.ApiClient.ModifySelfAsync(args);
        }
    }
}
