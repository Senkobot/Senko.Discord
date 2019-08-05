using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Internal;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public partial class DiscordClient
    {
        private void AttachHandlers()
        {
            Gateway.OnChannelCreate += UpdateChannelCacheAsync;
            Gateway.OnChannelUpdate += UpdateChannelCacheAsync;

            Gateway.OnChannelDelete += DeleteChannelCacheAsync;

            Gateway.OnGuildCreate += InsertGuildCacheAsync;
            Gateway.OnGuildUpdate += UpdateGuildCacheAsync;
            Gateway.OnGuildDelete += DeleteGuildCacheAsync;

            Gateway.OnGuildEmojiUpdate += UpdateEmojiCacheAsync;

            Gateway.OnGuildMemberAdd += InsertGuildMemberCacheAsync;
            Gateway.OnGuildMemberRemove += DeleteGuildMemberCacheAsync;
            Gateway.OnGuildMemberUpdate += UpdateGuildMemberCacheAsync;

            Gateway.OnGuildRoleCreate += UpdateRoleCacheAsync;
            Gateway.OnGuildRoleUpdate += UpdateRoleCacheAsync;
            Gateway.OnGuildRoleDelete += DeleteRoleCacheAsync;

            Gateway.OnUserUpdate += UpdateUserCacheAsync;
            Gateway.OnPresenceUpdate += UpdatePresenceAsync;

            Gateway.OnReady += OnReadyAsync;
        }

        private void DetachHandlers()
        {
            Gateway.OnChannelCreate -= UpdateChannelCacheAsync;
            Gateway.OnChannelUpdate -= UpdateChannelCacheAsync;

            Gateway.OnChannelDelete -= DeleteChannelCacheAsync;

            Gateway.OnGuildCreate -= InsertGuildCacheAsync;
            Gateway.OnGuildUpdate -= UpdateGuildCacheAsync;
            Gateway.OnGuildDelete -= DeleteGuildCacheAsync;

            Gateway.OnGuildEmojiUpdate -= UpdateEmojiCacheAsync;

            Gateway.OnGuildMemberAdd -= InsertGuildMemberCacheAsync;
            Gateway.OnGuildMemberRemove -= DeleteGuildMemberCacheAsync;
            Gateway.OnGuildMemberUpdate -= UpdateGuildMemberCacheAsync;

            Gateway.OnGuildRoleCreate -= UpdateRoleCacheAsync;
            Gateway.OnGuildRoleUpdate -= UpdateRoleCacheAsync;

            Gateway.OnGuildRoleDelete -= DeleteRoleCacheAsync;

            Gateway.OnUserUpdate -= UpdateUserCacheAsync;
            Gateway.OnPresenceUpdate -= UpdatePresenceAsync;

            Gateway.OnReady -= OnReadyAsync;
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

        private Task OnReadyAsync(GatewayReadyPacket ready)
        {
            _currentUserId = ready.CurrentUser.Id;

            KeyValuePair<string, DiscordGuildPacket>[] readyPackets = new KeyValuePair<string, DiscordGuildPacket>[ready.Guilds.Count()];

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
                await OnGuildMemberRolesUpdate(new DiscordGuildUser(cacheMember, this));
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
    }
}
