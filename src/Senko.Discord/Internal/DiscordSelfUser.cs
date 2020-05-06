using System;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public class DiscordSelfUser : DiscordUser, IDiscordSelfUser
    {
        public DiscordSelfUser(DiscordUserPacket user, IDiscordClient client)
            : base(user, client)
        { }

        public ValueTask<IDiscordChannel> GetDMChannelsAsync()
        {
            throw new NotImplementedException();
        }

        public async ValueTask ModifyAsync(Action<UserModifyArgs> modifyArgs)
        {
            var args = new UserModifyArgs();
            modifyArgs(args);
            await Client.ModifySelfAsync(args);
        }
    }
}
