﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Senko.Discord.Packets;
using Senko.Discord.Rest;

namespace Senko.Discord
{
    public interface IDiscordClient : IDisposable
    {
        ValueTask StartAsync();

        ValueTask StopAsync();

        IDiscordApiClient ApiClient { get; }

        IDiscordGateway Gateway { get; }

        ulong? CurrentUserId { get; set; }

        ValueTask<IDiscordTextChannel> CreateDMAsync(ulong userid);

        ValueTask<IDiscordRole> CreateRoleAsync(ulong guildId, CreateRoleArgs args = null);

        ValueTask<IDiscordRole> EditRoleAsync(ulong guildId, DiscordRolePacket role);

        ValueTask<IDiscordPresence> GetUserPresence(ulong userId, ulong? guildId = null);

        ValueTask<IDiscordRole> GetRoleAsync(ulong guildId, ulong roleId);

        ValueTask<IEnumerable<IDiscordRole>> GetRolesAsync(ulong guildId);

        ValueTask<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId);

        ValueTask<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null);

        ValueTask<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) where T : class, IDiscordChannel;

        ValueTask<IDiscordSelfUser> GetSelfAsync();

        ValueTask<IDiscordGuild> GetGuildAsync(ulong id);

        ValueTask<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId);

        ValueTask<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId);

        ValueTask<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji);

        ValueTask<IDiscordUser> GetUserAsync(ulong id);

        ValueTask SetGameAsync(int shardId, DiscordStatus status);

        ValueTask<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null);

        ValueTask<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message);

        ValueTask<IDiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, EditMessageArgs message);
    }
}