using System;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public class DiscordUser : IDiscordUser
    {
        private readonly DiscordUserPacket _user;
        private string _normalizedUsername;

        protected readonly IDiscordClient Client;

        public DiscordUser(DiscordUserPacket packet, IDiscordClient client)
        {
            Client = client;
            _user = packet;
        }

        public string Username
            => _user.Username;

        public string Discriminator
            => _user.Discriminator;

        public bool IsBot
            => _user.IsBot;

        public ulong Id
            => _user.Id;

        public DiscordUserPacket Packet => _user;

        public string AvatarId
            => _user.Avatar;

        public string GetAvatarUrl(ImageType type = ImageType.AUTO, ImageSize size = ImageSize.x256)
            => DiscordUtils.GetAvatarUrl(_user, type, size);

        public string Mention
            => $"<@{Id}>";

        public DateTimeOffset CreatedAt
            => this.GetCreationTime();

        public ValueTask<IDiscordPresence> GetPresenceAsync()
        {
            return Client.GetUserPresence(Id);
        }

        public async ValueTask<IDiscordTextChannel> GetDMChannelAsync()
        {
            var currentUser = await Client.GetSelfAsync();

            if (Id == currentUser.Id)
            {
                throw new InvalidOperationException("Can't create a DM channel with self.");
            }

            return await Client.CreateDMAsync(Id);
        }
    }
}