using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Caching;
using Senko.Discord.Internal;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public class DiscordPacketHandler : BaseDiscordPacketHandler
    {
        protected readonly ICacheClient CacheClient;

        public DiscordPacketHandler(IDiscordEventHandler eventHandler, IDiscordClient client, ICacheClient cacheClient)
            : base(eventHandler, client)
        {
            CacheClient = cacheClient;
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
                CacheClient.SetAllAsync(ready.Guilds.ToDictionary(g => CacheKey.Guild(g.Id), g => g)),
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

        private async Task UpdateGuildMemberCacheAsync(GuildMemberUpdateEventArgs member)
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

        private async Task UpdateGuildCacheAsync(DiscordGuildPacket guild)
        {
            var cache = await CacheClient.GetAsync<DiscordGuildPacket>(CacheKey.Guild(guild.Id));
            var cacheGuild = cache.HasValue ? cache.Value : new DiscordGuildPacket();

            cacheGuild.OverwriteContext(guild);

            await CacheClient.SetAsync(CacheKey.Guild(guild.Id), cacheGuild);
        }

        private Task InsertGuildCacheAsync(DiscordGuildPacket guild)
        {
            guild.Members.RemoveAll(x => x == null);

            return Task.WhenAll(
                new Task[]
                    {
                        CacheClient.SetAsync(CacheKey.Guild(guild.Id), guild),
                        CacheClient.SetAllAsync(guild.Members.ToDictionary(p => CacheKey.User(p.User.Id), p => p.User))
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
                return CacheClient.SetAsync(CacheKey.Channel(), channel);
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

        public override async Task OnChannelCreate(DiscordChannelPacket packet)
        {
            await UpdateChannelCacheAsync(packet);
            await base.OnChannelCreate(packet);
        }

        public override async Task OnChannelUpdate(DiscordChannelPacket packet)
        {
            await UpdateChannelCacheAsync(packet);
            await base.OnChannelUpdate(packet);
        }

        public override async Task OnChannelDelete(DiscordChannelPacket packet)
        {
            await DeleteChannelCacheAsync(packet);
            await base.OnChannelDelete(packet);
        }

        public override async Task OnGuildCreate(DiscordGuildPacket packet)
        {
            await InsertGuildCacheAsync(packet);
            await base.OnGuildCreate(packet);
        }

        public override async Task OnGuildUpdate(DiscordGuildPacket packet)
        {
            await UpdateGuildCacheAsync(packet);
            await base.OnGuildUpdate(packet);
        }

        public override async Task OnGuildDelete(DiscordGuildUnavailablePacket packet)
        {
            await DeleteGuildCacheAsync(packet);
            await base.OnGuildDelete(packet);
        }

        public override async Task OnGuildMemberAdd(DiscordGuildMemberPacket packet)
        {
            await InsertGuildMemberCacheAsync(packet);
            await base.OnGuildMemberAdd(packet);
        }

        public override async Task OnGuildMemberUpdate(GuildMemberUpdateEventArgs packet)
        {
            await UpdateGuildMemberCacheAsync(packet);
            await base.OnGuildMemberUpdate(packet);
        }

        public override async Task OnGuildMemberRemove(GuildIdUserArgs packet)
        {
            await DeleteGuildMemberCacheAsync(packet.GuildId, packet.User);
            await base.OnGuildMemberRemove(packet);
        }

        public override async Task OnGuildEmojiUpdate(GuildEmojisUpdateEventArgs packet)
        {
            await UpdateEmojiCacheAsync(packet.GuildId, packet.Emojis);
            await base.OnGuildEmojiUpdate(packet);
        }

        public override async Task OnGuildRoleCreate(RoleEventArgs packet)
        {
            await UpdateRoleCacheAsync(packet.GuildId, packet.Role);
            await base.OnGuildRoleCreate(packet);
        }

        public override async Task OnGuildRoleUpdate(RoleEventArgs packet)
        {
            await UpdateRoleCacheAsync(packet.GuildId, packet.Role);
            await base.OnGuildRoleUpdate(packet);
        }

        public override async Task OnGuildRoleDelete(RoleDeleteEventArgs packet)
        {
            await DeleteRoleCacheAsync(packet.GuildId, packet.RoleId);
            await base.OnGuildRoleDelete(packet);
        }

        public override async Task OnPresenceUpdate(DiscordPresencePacket packet)
        {
            await UpdatePresenceAsync(packet);
            await base.OnPresenceUpdate(packet);
        }

        public override async Task OnReady(GatewayReadyPacket packet)
        {
            await InitializeCacheAsync(packet);
            await base.OnReady(packet);
        }

        public override async Task OnUserUpdate(DiscordUserPacket packet)
        {
            await UpdateUserCacheAsync(packet);
            await base.OnUserUpdate(packet);
        }

        private IEnumerable<Task> StoreListAsync<T>(string listName, Func<ulong, string> cacheName, IReadOnlyList<T> packets)
            where T : ISnowflake
        {
            return new Task[]
            {
                CacheClient.SetAsync(listName, packets.Select(p => p.Id).ToArray()),
                CacheClient.SetAllAsync(packets.ToDictionary(p => cacheName(p.Id), p => p))
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
