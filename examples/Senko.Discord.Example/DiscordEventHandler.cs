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

        public Task OnGuildRoleDeleted(ulong guildId, ulong roleId)
        {
            return Task.CompletedTask;
        }

        public async Task OnMessageCreate(IDiscordMessage message)
        {
            if (message.Content == "!ping")
            {
                await _client.SendMessageAsync(message.ChannelId, "Pong");
            }
        }

        #region Unimplemented methods

        public Task OnGuildJoin(IDiscordGuild guild)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildUpdate(IDiscordGuild guild)
        {
            return Task.CompletedTask;
        }

        public Task OnUserUpdate(IDiscordUser user)
        {
            return Task.CompletedTask;
        }

        public Task OnChannelCreate(IDiscordChannel channel)
        {
            return Task.CompletedTask;
        }

        public Task OnChannelUpdate(IDiscordChannel channel)
        {
            return Task.CompletedTask;
        }

        public Task OnChannelDelete(IDiscordChannel channel)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildUnavailable(ulong guildId)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildLeave(ulong guildId)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberDelete(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberUpdate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberCreate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildRoleCreate(ulong guildId, IDiscordRole role)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildRoleUpdate(ulong guildId, IDiscordRole role)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageUpdate(IDiscordMessage message)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageDeleted(ulong channelId, ulong messageId)
        {
            return Task.CompletedTask;
        }

        public Task OnGuildMemberRolesUpdate(IDiscordGuildUser member)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
