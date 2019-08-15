using System.Linq;
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
            var channel = Client.GetChannelFromPacket(packet);

            return EventHandler.OnChannelCreate(channel);
        }

        public virtual Task OnChannelUpdate(DiscordChannelPacket packet)
        {
            var channel = Client.GetChannelFromPacket(packet);

            return EventHandler.OnChannelCreate(channel);
        }

        public virtual Task OnChannelDelete(DiscordChannelPacket packet)
        {
            var channel = Client.GetChannelFromPacket(packet);

            return EventHandler.OnChannelCreate(channel);
        }

        public virtual Task OnGuildCreate(DiscordGuildPacket packet)
        {
            var guild = new DiscordGuild(packet, Client);

            return EventHandler.OnGuildJoin(guild);
        }

        public virtual Task OnGuildUpdate(DiscordGuildPacket packet)
        {
            var guild = new DiscordGuild(packet, Client);

            return EventHandler.OnGuildUpdate(guild);
        }

        public virtual Task OnGuildDelete(DiscordGuildUnavailablePacket packet)
        {
            return packet.IsUnavailable.GetValueOrDefault(false)
                ? EventHandler.OnGuildUnavailable(packet.GuildId)
                : EventHandler.OnGuildLeave(packet.GuildId);
        }

        public virtual Task OnGuildMemberAdd(DiscordGuildMemberPacket packet)
        {
            var member = new DiscordGuildUser(packet, Client);

            return EventHandler.OnGuildMemberCreate(member);
        }

        public virtual async Task OnGuildMemberRemove(GuildIdUserArgs packet)
        {
            var member = await Client.GetGuildUserAsync(packet.User.Id, packet.GuildId);

            if (member != null)
            {
                await EventHandler.OnGuildMemberDelete(member);
            }
        }

        public virtual async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs packet)
        {
            var member = await Client.GetGuildUserAsync(packet.User.Id, packet.GuildId);

            if (member != null)
            {
                await EventHandler.OnGuildMemberUpdate(member);
            }
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
            var role = new DiscordRole(packet.Role, Client);

            return EventHandler.OnGuildRoleCreate(packet.GuildId, role);
        }

        public virtual Task OnGuildRoleUpdate(RoleEventArgs packet)
        {
            var role = new DiscordRole(packet.Role, Client);

            return EventHandler.OnGuildRoleUpdate(packet.GuildId, role);
        }

        public virtual Task OnGuildRoleDelete(RoleDeleteEventArgs packet)
        {
            return EventHandler.OnGuildRoleDeleted(packet.GuildId, packet.RoleId);
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
            return EventHandler.OnMessageDeleted(packet.ChannelId, packet.MessageId);
        }

        public virtual Task OnMessageDeleteBulk(MessageBulkDeleteEventArgs packet)
        {
            return Task.WhenAll(packet.MessagesDeleted.Select(id => EventHandler.OnMessageDeleted(packet.ChannelId, id)));
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
