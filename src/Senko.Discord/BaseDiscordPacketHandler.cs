using System.Threading.Tasks;
using Senko.Discord.Gateway;
using Senko.Discord.Internal;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public abstract class BaseDiscordPacketHandler : IDiscordPacketHandler
    {
        protected readonly IDiscordEventHandler EventHandler;
        protected readonly IDiscordClient Client;

        protected BaseDiscordPacketHandler(IDiscordEventHandler eventHandler, IDiscordClient client)
        {
            EventHandler = eventHandler;
            Client = client;
        }

        public virtual Task OnChannelCreate(DiscordChannelPacket packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnChannelUpdate(DiscordChannelPacket packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnChannelDelete(DiscordChannelPacket packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildCreate(DiscordGuildPacket packet)
        {
            var guild = new DiscordGuild(packet, Client);

            return EventHandler.OnGuildJoin(guild);
        }

        public virtual Task OnGuildUpdate(DiscordGuildPacket packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildDelete(DiscordGuildUnavailablePacket packet)
        {
            if (packet.IsUnavailable.GetValueOrDefault(false))
            {
                return EventHandler.OnGuildUnavailable(packet.GuildId);
            }

            return EventHandler.OnGuildLeave(packet.GuildId);
        }

        public virtual Task OnGuildMemberAdd(DiscordGuildMemberPacket packet)
        {
            var member = new DiscordGuildUser(packet, Client);

            return EventHandler.OnGuildMemberCreate(member);
        }

        public virtual async Task OnGuildMemberRemove(GuildIdUserArgs packet)
        {
            var member = await Client.GetGuildUserAsync(packet.User.Id, packet.GuildId);

            if (member == null)
            {
                return;
            }

            await EventHandler.OnGuildMemberDelete(member);
        }

        public virtual Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildBanAdd(GuildIdUserArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildBanRemove(GuildIdUserArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildEmojiUpdate(GuildEmojisUpdateEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildRoleCreate(RoleEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildRoleUpdate(RoleEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnGuildRoleDelete(RoleDeleteEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnMessageCreate(DiscordMessagePacket packet)
        {
            var message = new DiscordMessage(packet, Client);

            return EventHandler.OnMessageCreate(message);
        }

        public virtual Task OnMessageUpdate(DiscordMessagePacket packet)
        {
            var message = new DiscordMessage(packet, Client);

            return EventHandler.OnMessageUpdate(message);
        }

        public virtual Task OnMessageDelete(MessageDeleteArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnMessageDeleteBulk(MessageBulkDeleteEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPresenceUpdate(DiscordPresencePacket packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnReady(GatewayReadyPacket packet)
        {
            Client.CurrentUserId = packet.CurrentUser.Id;

            return Task.CompletedTask;
        }

        public virtual Task OnResume(GatewayReadyPacket packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnTypingStart(TypingStartEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnUserUpdate(DiscordUserPacket packet)
        {
            var user = new DiscordUser(packet, Client);

            return EventHandler.OnUserUpdate(user);
        }
    }
}
