using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.Logging;
using Senko.Discord.Internal;
using Senko.Discord.Packets;
using Senko.Discord.Rest;

namespace Senko.Discord
{
    public partial class DiscordClient : BaseDiscordClient
    {
        private readonly ILogger<DiscordClient> _logger;

        public DiscordClient(
            IDiscordApiClient apiClient,
            IDiscordGateway gateway,
            ICacheClient cacheClient,
            ILogger<DiscordClient> logger,
            IServiceProvider provider
        )  : base(apiClient, gateway)
        {
            CacheClient = cacheClient;
            _logger = logger;
        }

        public ICacheClient CacheClient { get; }

        private static bool ValidateCache(DiscordChannelPacket p)
        {
            if (!p.GuildId.HasValue && (p.Type == ChannelType.GUILDTEXT
                || p.Type == ChannelType.GUILDNEWS
                || p.Type == ChannelType.CATEGORY
                || p.Type == ChannelType.GUILDVOICE))
            {
                return false;
            }

            return !string.IsNullOrEmpty(p.Name);
        }

        private static bool ValidateCache(DiscordGuildMemberPacket p) => ValidateCache(p.User);

        private static bool ValidateCache(DiscordRolePacket p) => !string.IsNullOrEmpty(p.Name);

        private static bool ValidateCache(DiscordGuildPacket p) => !string.IsNullOrEmpty(p.Name);

        private static bool ValidateCache(DiscordUserPacket p) => !string.IsNullOrEmpty(p.Username);

        protected override ValueTask<DiscordChannelPacket> GetChannelPacketAsync(ulong id)
        {
            return GetOrLoadAsync(
                CacheKey.Channel(id),
                () => ApiClient.GetChannelAsync(id),
                ValidateCache
            );
        }

        protected override ValueTask<DiscordGuildMemberPacket[]> GetGuildMembersPacketAsync(ulong guildId)
        {
            return GetOrLoadListAsync(
                CacheKey.GuildMemberIdList(guildId),
                id => CacheKey.GuildMember(guildId, id),
                () => ApiClient.GetGuildMembersAsync(guildId),
                ValidateCache,
                packets => new Task[]
                {
                    CacheClient.SetAllAsync(packets.ToDictionary(p => CacheKey.User(p.User.Id), p => p.User))
                }
            );
        }

        protected override ValueTask<DiscordChannelPacket[]> GetGuildChannelPacketsAsync(ulong guildId)
        {
            return GetOrLoadListAsync(
                CacheKey.ChannelIdList(guildId), 
                CacheKey.Channel, 
                () => ApiClient.GetChannelsAsync(guildId),
                ValidateCache
            );
        }

        protected override ValueTask<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId)
        {
            return GetOrLoadAsync(
                CacheKey.GuildMember(guildId, userId),
                () => ApiClient.GetGuildUserAsync(userId, guildId),
                ValidateCache,
                p => CacheClient.SetAsync(CacheKey.User(p.User.Id), p.User)
            );
        }

        protected override ValueTask<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId)
        {
            return GetOrLoadAsync(
                CacheKey.GuildRole(guildId, roleId),
                () => ApiClient.GetRoleAsync(roleId, guildId),
                ValidateCache
            );
        }

        protected override ValueTask<DiscordRolePacket[]> GetRolePacketsAsync(ulong guildId)
        {
            return GetOrLoadListAsync(
                CacheKey.GuildRoleIdList(guildId),
                id => CacheKey.GuildRole(guildId, id),
                () => ApiClient.GetRolesAsync(guildId),
                ValidateCache
            );
        }

        protected override ValueTask<DiscordGuildPacket> GetGuildPacketAsync(ulong id)
        {
            return GetOrLoadAsync(
                CacheKey.Guild(id),
                () => ApiClient.GetGuildAsync(id),
                ValidateCache
            );
        }

        protected override ValueTask<DiscordUserPacket> GetUserPacketAsync(ulong id)
        {
            return GetOrLoadAsync(
                CacheKey.User(id),
                () => ApiClient.GetUserAsync(id),
                ValidateCache
            );
        }

        protected override ValueTask<DiscordUserPacket> GetCurrentUserPacketAsync()
        {
            if (!CurrentUserId.HasValue)
            {
                return default;
            }

            return GetOrLoadAsync(
                CacheKey.User(CurrentUserId.Value),
                ApiClient.GetCurrentUserAsync,
                ValidateCache
            );
        }

        private async ValueTask<T[]> GetOrLoadListAsync<T>(
            string listName,
            Func<ulong, string> cacheName,
            Func<ValueTask<T[]>> retrieveFunc,
            Func<T, bool> cacheValidator,
            Func<IReadOnlyList<T>, IEnumerable<Task>> storeFunc = null
        ) where T : ISnowflake
        {
            var cache = await CacheClient.GetAsync<ulong[]>(listName);

            if (cache.HasValue)
            {
                var items = await CacheClient.GetAllAsync<T>(cache.Value.Select(cacheName));

                if (items.All(i => i.Value.HasValue && cacheValidator(i.Value.Value)))
                {
                    return items.Values.Select(c => c.Value).ToArray();
                }

                _logger.LogDebug("Ignoring invalid cache (list, {Type})", typeof(T).Name);
            }

            var packets = await retrieveFunc();

            if (packets.Length > 0)
            {
                var tasks = StoreListAsync(listName, cacheName, packets);

                if (storeFunc != null)
                {
                    tasks = tasks.Union(storeFunc(packets));
                }

                await Task.WhenAll(tasks);
            }

            return packets;
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

        private async ValueTask<T> GetOrLoadAsync<T>(
            string cacheName,
            Func<ValueTask<T>> retrieveFunc,
            Func<T, bool> cacheValidator,
            Func<T, Task> storeFunc = null
        ) where T : ISnowflake
        {
            var cache = await CacheClient.GetAsync<T>(cacheName);

            if (cache.HasValue)
            {
                if (cacheValidator(cache.Value))
                {
                    return cache.Value;
                }

                _logger.LogDebug("Ignoring invalid cache ({Id}, {Type})", cache.Value.Id, typeof(T).Name);
            }

            var packet = await retrieveFunc();

            if (packet != null)
            {
                await CacheClient.SetAsync(cacheName, packet);

                if (storeFunc != null)
                {
                    await storeFunc(packet);
                }
            }

            return packet;
        }
    }
}
