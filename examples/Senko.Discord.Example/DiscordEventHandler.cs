using System;
using System.Collections.Generic;
using System.Linq;
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
                // Command !ping
                // Replies with the message "Pong".
                case "!ping":
                    await _client.SendMessageAsync(message.ChannelId, "Pong");
                    break;
                
                // Command !embed
                // Replies with a example embed.
                case "!embed":
                    await _client.SendMessageAsync(message.ChannelId, null, new DiscordEmbed
                    {
                        Title = "Example",
                        Description = "Example embed"
                    });
                    break;
                
                // Command !users
                // Show all users of the guild with their normalized name.
                case "!users" when message.GuildId.HasValue:
                {
                    var memberNames = (await _client.GetGuildMemberNamesAsync(message.GuildId.Value))
                        .Select(n => $"- {n.Nickname ?? n.Username} ({n.NormalizedNickname ?? n.NormalizedUsername})");
                    var response = string.Join("\n", memberNames);
                    
                    await _client.SendMessageAsync(message.ChannelId, response);
                    break;
                }
                
                // Command !spam
                case "!spam" when message.GuildId.HasValue:
                {
                    for (var i = 0; i < 10; i++)
                    {
                        await _client.SendMessageAsync(message.ChannelId, $"Message {i}");
                    }
                    break;
                }
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
