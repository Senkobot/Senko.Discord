using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.Logging;
using Senko.Discord.Packets;
using Senko.Discord.Rest;

namespace Senko.Discord.Example
{
    public class DiscordEventHandler : IDiscordEventHandler
    {
        private readonly IDiscordClient _client;

        public DiscordEventHandler(IDiscordClient client)
        {
            _client = client;
        }

        public async ValueTask OnMessageCreate(IDiscordMessage message)
        {
            switch (message.Content)
            {
                case "!ping":
                    await _client.SendMessageAsync(message.ChannelId, "Pong");
                    break;
                case "!embed":
                    await _client.SendMessageAsync(message.ChannelId, null, new DiscordEmbed
                    {
                        Title = "Example",
                        Description = "Example embed"
                    });
                    break;
            }
        }

        #region Unimplemented methods

        public ValueTask OnGuildRoleDeleted(ulong guildId, IDiscordRole role)
        {
            return default;
        }

        public ValueTask OnGuildJoin(IDiscordGuild guild)
        {
            return default;
        }

        public ValueTask OnGuildUpdate(IDiscordGuild guild)
        {
            return default;
        }

        public ValueTask OnUserUpdate(IDiscordUser user)
        {
            return default;
        }

        public ValueTask OnChannelCreate(IDiscordChannel channel)
        {
            return default;
        }

        public ValueTask OnChannelUpdate(IDiscordChannel channel)
        {
            return default;
        }

        public ValueTask OnChannelDelete(IDiscordChannel channel)
        {
            return default;
        }

        public ValueTask OnGuildUnavailable(ulong guildId)
        {
            return default;
        }

        public ValueTask OnGuildLeave(ulong guildId)
        {
            return default;
        }

        public ValueTask OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return default;
        }

        public ValueTask OnGuildMemberUpdate(IDiscordGuildUser member)
        {
            return default;
        }

        public ValueTask OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return default;
        }

        public ValueTask OnGuildRoleCreate(ulong guildId, IDiscordRole role)
        {
            return default;
        }

        public ValueTask OnGuildRoleUpdate(ulong guildId, IDiscordRole role)
        {
            return default;
        }

        public ValueTask OnMessageUpdate(IDiscordMessage message)
        {
            return default;
        }

        public ValueTask OnMessageDeleted(ulong channelId, ulong messageId)
        {
            return default;
        }

        public ValueTask OnMessageEmojiCreated(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return default;
        }

        public ValueTask OnMessageEmojiDeleted(ulong? guildId, ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return default;
        }

        public ValueTask OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return default;
        }

        #endregion
    }
}
