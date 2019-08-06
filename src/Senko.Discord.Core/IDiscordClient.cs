using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Senko.Discord.Packets;
using Senko.Discord.Rest;

namespace Senko.Discord
{
    public interface IDiscordClient : IDisposable
    {
        Task StartAsync();

        Task StopAsync();

        IDiscordApiClient ApiClient { get; }

        IDiscordGateway Gateway { get; }

        ulong? CurrentUserId { get; set; }

        Task<IDiscordTextChannel> CreateDMAsync(ulong userid);

        Task<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null);

        Task<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role);

        Task<IDiscordPresence> GetUserPresence(ulong userId, ulong? guildId = null);

        Task<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId);

        Task<IEnumerable<IDiscordRole>> GetRolesAsync(ulong guildId);

        Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId);

        Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null);

        Task<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) where T : class, IDiscordChannel;

        Task<IDiscordSelfUser> GetSelfAsync();

        Task<IDiscordGuild> GetGuildAsync(ulong id);

        Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId);

        Task<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId);

        Task<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

        Task<IDiscordUser> GetUserAsync(ulong id);

        Task SetGameAsync(int shardId, DiscordStatus status);

        Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null);

        Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message);

        Task<IDiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs message);
    }
}