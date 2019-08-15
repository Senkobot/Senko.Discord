using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Caching;
using Senko.Discord.Gateway;
using Senko.Discord.Internal;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public class DiscordPacketHandler : IDiscordPacketHandler
    {
        protected readonly IDiscordEventHandler EventHandler;
        protected readonly IDiscordClient Client;
        protected readonly ICacheClient CacheClient;

        public DiscordPacketHandler(IDiscordEventHandler eventHandler, IDiscordClient client, ICacheClient cacheClient)
        {
            EventHandler = eventHandler;
            Client = client;
            CacheClient = cacheClient;
        }

        public async Task OnChannelCreate(DiscordChannelPacket packet)
        {
            await UpdateChannelCacheAsync(packet);
            
            var channel = Client.GetChannelFromPacket(packet);

            await EventHandler.OnChannelCreate(channel);
        }

        public async Task OnChannelUpdate(DiscordChannelPacket packet)
        {
            await UpdateChannelCacheAsync(packet);
            
            var channel = Client.GetChannelFromPacket(packet);

            await EventHandler.OnChannelCreate(channel);
        }

        public async Task OnChannelDelete(DiscordChannelPacket packet)
        {
            var channel = await Client.GetChannelAsync(packet.Id, packet.GuildId);

            if (channel != null)
            {
                await EventHandler.OnChannelDelete(channel);
            }

            await DeleteChannelCacheAsync(packet);
        }

        public async Task OnGuildCreate(DiscordGuildPacket packet)
        {
            await InsertGuildCacheAsync(packet);
            var guild = new DiscordGuild(packet, Client);

            await EventHandler.OnGuildJoin(guild);
        }

        public async Task OnGuildUpdate(DiscordGuildPacket packet)
        {
            var updatedPacket = await UpdateGuildCacheAsync(packet);
            var guild = new DiscordGuild(updatedPacket, Client);

            await EventHandler.OnGuildUpdate(guild);
        }

        public async Task OnGuildDelete(DiscordGuildUnavailablePacket packet)
        {
            if (packet.IsUnavailable.GetValueOrDefault(false))
            {
                await EventHandler.OnGuildUnavailable(packet.GuildId);
            }
            else
            {
                await EventHandler.OnGuildLeave(packet.GuildId);
            }

            await DeleteGuildCacheAsync(packet);
        }

        public async Task OnGuildMemberAdd(DiscordGuildMemberPacket packet)
        {
            await InsertGuildMemberCacheAsync(packet);
            
            var member = new DiscordGuildUser(packet, Client);

            await EventHandler.OnGuildMemberCreate(member);
        }

        public async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs packet)
        {
            var updatedPacket = await UpdateGuildMemberCacheAsync(packet);
            var member = new DiscordGuildUser(updatedPacket, Client);

            await EventHandler.OnGuildMemberUpdate(member);
        }

        public Task OnGuildBanAdd(GuildIdUserArgs packet)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildBanRemove(GuildIdUserArgs packet)
        {
            return Task.CompletedTask;
        }

        public async Task OnGuildMemberRemove(GuildIdUserArgs packet)
        {
            var member = await Client.GetGuildUserAsync(packet.User.Id, packet.GuildId);

            if (member != null)
            {
                await EventHandler.OnGuildMemberDelete(member);
            }

            await DeleteGuildMemberCacheAsync(packet.GuildId, packet.User);
        }

        public Task OnGuildEmojiUpdate(GuildEmojisUpdateEventArgs packet)
        {
            return UpdateEmojiCacheAsync(packet.GuildId, packet.Emojis);
        }

        public async Task OnGuildRoleCreate(RoleEventArgs packet)
        {
            await UpdateRoleCacheAsync(packet.GuildId, packet.Role);
            
            var role = new DiscordRole(packet.Role, Client);

            await EventHandler.OnGuildRoleCreate(packet.GuildId, role);
        }

        public async Task OnGuildRoleUpdate(RoleEventArgs packet)
        {
            await UpdateRoleCacheAsync(packet.GuildId, packet.Role);

            var role = new DiscordRole(packet.Role, Client);

            await EventHandler.OnGuildRoleUpdate(packet.GuildId, role);
        }

        public async Task OnGuildRoleDelete(RoleDeleteEventArgs packet)
        {
            var role = await Client.GetRoleAsync(packet.GuildId, packet.RoleId);

            if (role != null)
            {
                await EventHandler.OnGuildRoleDeleted(packet.GuildId, role);
            }

            await DeleteRoleCacheAsync(packet.GuildId, packet.RoleId);
        }

        public Task OnMessageCreate(DiscordMessagePacket packet)
        {
            var message = new DiscordMessage(packet, Client);

            return EventHandler.OnMessageCreate(message);
        }

        public Task OnMessageUpdate(DiscordMessagePacket packet)
        {
            var message = new DiscordMessage(packet, Client);

            return EventHandler.OnMessageUpdate(message);
        }

        public Task OnMessageDelete(MessageDeleteArgs packet)
        {
            return EventHandler.OnMessageDeleted(packet.ChannelId, packet.MessageId);
        }

        public Task OnMessageDeleteBulk(MessageBulkDeleteEventArgs packet)
        {
            return Task.WhenAll(packet.MessagesDeleted.Select(id => EventHandler.OnMessageDeleted(packet.ChannelId, id)));
        }

        public Task OnPresenceUpdate(DiscordPresencePacket packet)
        {
            return UpdatePresenceAsync(packet);
        }

        public Task OnReady(GatewayReadyPacket packet)
        {
            Client.CurrentUserId = packet.CurrentUser.Id;

            return InitializeCacheAsync(packet);
        }

        public Task OnResume(GatewayReadyPacket packet)
        {
            return Task.CompletedTask;
        }

        public Task OnTypingStart(TypingStartEventArgs packet)
        {
            return Task.CompletedTask;
        }

        public async Task OnUserUpdate(DiscordUserPacket packet)
        {
            await UpdateUserCacheAsync(packet);
            
            var user = new DiscordUser(packet, Client);

            await EventHandler.OnUserUpdate(user);
        }

        private async Task UpdateEmojiCacheAsync(ulong guildId, DiscordEmoji[] emojis)
        {
            var cacheKey = CacheKey.Guild(guildId);
            var cache = await CacheClient.GetAsync<DiscordGuildPacket>(cacheKey);

            if (!cache.HasValue)
            {
                return;
            }

            var guild = cache.Value;
            guild.Emojis = emojis;
            await CacheClient.SetAsync(cacheKey, guild);
        }

        private Task DeleteRoleCacheAsync(ulong guildId, ulong roleId)
        {
            return CacheClient.RemoveAsync(CacheKey.GuildRole(guildId, roleId));
        }

        private Task UpdateRoleCacheAsync(ulong guildId, DiscordRolePacket role)
        {
            return CacheClient.SetAsync(CacheKey.GuildRole(guildId, role.Id), role);
        }

        private Task InitializeCacheAsync(GatewayReadyPacket ready)
        {
            var readyPackets = new KeyValuePair<string, DiscordGuildPacket>[ready.Guilds.Length];

            for (int i = 0, max = readyPackets.Length; i < max; i++)
            {
                readyPackets[i] = new KeyValuePair<string, DiscordGuildPacket>(ready.Guilds[i].Id.ToString(), ready.Guilds[i]);
            }

            return Task.WhenAll(
                CacheClient.SetAllAsync(ready.Guilds.GroupBy(g => g.Id).ToDictionary(g => CacheKey.Guild(g.Key), g => g.First())),
                CacheClient.SetAsync(CacheKey.User(ready.CurrentUser.Id), ready.CurrentUser)
            );
        }

        private Task UpdateUserCacheAsync(DiscordUserPacket user)
        {
            return CacheClient.SetAsync(CacheKey.User(user.Id), user);
        }

        private async Task UpdatePresenceAsync(DiscordPresencePacket packet)
        {
            var cacheKey = CacheKey.User(packet.User.Id);
            var cache = await CacheClient.GetAsync<DiscordUserPacket>(cacheKey);

            if (!cache.HasValue)
            {
                return;
            }

            var user = cache.Value;
            var updated = false;

            if (packet.User.Username != null)
            {
                user.Username = packet.User.Username;
                updated = true;
            }

            if (packet.User.Avatar != null)
            {
                user.Avatar = packet.User.Avatar;
                updated = true;
            }

            if (packet.User.Discriminator != null)
            {
                user.Discriminator = packet.User.Discriminator;
                updated = true;
            }

            if (packet.User.Email != null)
            {
                user.Discriminator = packet.User.Email;
                updated = true;
            }

            if (!updated)
            {
                return;
            }

            await CacheClient.SetAsync(cacheKey, user);

            if (packet.GuildId.HasValue)
            {
                var guildCacheKey = CacheKey.GuildMember(packet.GuildId.Value, user.Id);
                var guildCache = await CacheClient.GetAsync<DiscordGuildMemberPacket>(guildCacheKey);

                if (!guildCache.HasValue)
                {
                    return;
                }

                var guildMember = guildCache.Value;

                guildMember.User = user;

                await CacheClient.SetAsync(guildCacheKey, guildMember);
            }
        }

        private async Task<DiscordGuildMemberPacket> UpdateGuildMemberCacheAsync(GuildMemberUpdateEventArgs member)
        {
            var key = CacheKey.GuildMember(member.GuildId, member.User.Id);
            var cache = await CacheClient.GetAsync<DiscordGuildMemberPacket>(key);
            var cacheMember = cache.HasValue ? cache.Value : new DiscordGuildMemberPacket();
            var rolesEdited = member.RoleIds.Length != cacheMember.Roles.Count ||
                             !member.RoleIds.All(cacheMember.Roles.Contains);

            cacheMember.User = member.User;
            cacheMember.Roles = member.RoleIds.ToList();
            cacheMember.Nickname = member.Nickname;

            await CacheClient.SetAsync(key, cacheMember);

            if (rolesEdited)
            {
                await EventHandler.OnGuildMemberRolesUpdate(new DiscordGuildUser(cacheMember, Client));
            }

            return cacheMember;
        }

        private Task InsertGuildMemberCacheAsync(DiscordGuildMemberPacket member)
        {
            return AddAsync(
                CacheKey.GuildMemberIdList(member.GuildId),
                CacheKey.GuildMember(member.GuildId, member.User.Id),
                member
            );
        }

        private Task DeleteGuildMemberCacheAsync(ulong guildId, DiscordUserPacket user)
        {
            return RemoveAsync(
                CacheKey.GuildMemberIdList(guildId),
                CacheKey.GuildMember(guildId, user.Id),
                user.Id
            );
        }

        private Task DeleteGuildCacheAsync(DiscordGuildUnavailablePacket unavailableGuild)
        {
            return CacheClient.RemoveAsync(CacheKey.Guild(unavailableGuild.GuildId));
        }

        private async Task<DiscordGuildPacket> UpdateGuildCacheAsync(DiscordGuildPacket guild)
        {
            var cache = await CacheClient.GetAsync<DiscordGuildPacket>(CacheKey.Guild(guild.Id));
            var cacheGuild = cache.HasValue ? cache.Value : new DiscordGuildPacket();

            cacheGuild.OverwriteContext(guild);

            await CacheClient.SetAsync(CacheKey.Guild(guild.Id), cacheGuild);

            return cacheGuild;
        }

        private Task InsertGuildCacheAsync(DiscordGuildPacket guild)
        {
            guild.Members.RemoveAll(x => x == null);

            return Task.WhenAll(
                new Task[]
                    {
                        CacheClient.SetAsync(CacheKey.Guild(guild.Id), guild),
                        CacheClient.SetAllAsync(guild.Members
                            .GroupBy(u => u.User.Id)
                            .ToDictionary(g => CacheKey.User(g.Key), g => g.First().User))
                    }
                    .Union(StoreListAsync(
                        CacheKey.ChannelIdList(guild.Id),
                        CacheKey.Channel,
                        guild.Channels)
                    )
                    .Union(StoreListAsync(
                        CacheKey.GuildRoleIdList(guild.Id),
                        id => CacheKey.GuildRole(guild.Id, id),
                        guild.Roles)
                    )
                    .Union(StoreListAsync(
                        CacheKey.GuildMemberIdList(guild.Id),
                        id => CacheKey.GuildMember(guild.Id, id),
                        guild.Members)
                    )
            );
        }

        private Task UpdateChannelCacheAsync(DiscordChannelPacket channel)
        {
            if (!channel.GuildId.HasValue)
            {
                return CacheClient.SetAsync(CacheKey.Channel(channel.Id), channel);
            }

            return AddAsync(
                CacheKey.ChannelIdList(channel.GuildId.Value),
                CacheKey.Channel(channel.Id),
                channel
            );
        }

        private Task DeleteChannelCacheAsync(DiscordChannelPacket channel)
        {
            if (!channel.GuildId.HasValue)
            {
                return CacheClient.RemoveAsync(CacheKey.Channel());
            }

            return RemoveAsync(
                CacheKey.ChannelIdList(channel.GuildId.Value),
                CacheKey.Channel(channel.Id),
                channel.Id
            );
        }

        private IEnumerable<Task> StoreListAsync<T>(string listName, Func<ulong, string> cacheName, IReadOnlyList<T> packets)
            where T : ISnowflake
        {
            return new Task[]
            {
                CacheClient.SetAsync(listName, packets.Select(p => p.Id).ToArray()),
                CacheClient.SetAllAsync(packets.GroupBy(p => p.Id).ToDictionary(p => cacheName(p.Key), p => p.First()))
            };
        }

        private async Task AddAsync<T>(string listName, string cacheName, T item)
            where T : ISnowflake
        {
            var cache = await CacheClient.GetAsync<List<ulong>>(listName);

            if (cache.HasValue)
            {
                var ids = cache.Value;
                ids.Add(item.Id);
                await CacheClient.SetAsync(listName, ids);
            }

            await CacheClient.SetAsync(cacheName, item);
        }

        private async Task RemoveAsync(string listName, string cacheName, ulong id)
        {
            var cache = await CacheClient.GetAsync<List<ulong>>(listName);

            if (cache.HasValue)
            {
                var ids = cache.Value;
                ids.Remove(id);
                await CacheClient.SetAsync(listName, ids);
            }

            await CacheClient.RemoveAsync(cacheName);
        }
    }
}
