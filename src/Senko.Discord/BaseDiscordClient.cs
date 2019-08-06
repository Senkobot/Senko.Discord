using Senko.Discord.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord.Gateway;
using Senko.Discord.Packets;
using Senko.Discord.Rest;

namespace Senko.Discord
{
	public abstract class BaseDiscordClient : IDiscordClient
    {
        protected BaseDiscordClient(IDiscordApiClient apiClient, IDiscordGateway gateway)
		{
            ApiClient = apiClient;
			Gateway = gateway;
        }

        public IDiscordApiClient ApiClient { get; }

        public IDiscordGateway Gateway { get; }

        public ulong? CurrentUserId { get; set; }

        public Task StartAsync()
            => Gateway.StartAsync();

        public Task StopAsync()
            => Gateway.StopAsync();

        protected abstract Task<DiscordUserPacket> GetCurrentUserPacketAsync();

        protected abstract Task<DiscordUserPacket> GetUserPacketAsync(ulong id);

        protected abstract Task<DiscordGuildPacket> GetGuildPacketAsync(ulong id);

        protected abstract Task<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId);

        protected abstract Task<DiscordGuildMemberPacket[]> GetGuildMembersPacketAsync(ulong guildId);

        protected abstract Task<DiscordChannelPacket[]> GetGuildChannelPacketsAsync(ulong guildId);

        protected abstract Task<DiscordRolePacket[]> GetRolePacketsAsync(ulong guildId);

        protected abstract Task<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId);

        protected abstract Task<DiscordChannelPacket> GetChannelPacketAsync(ulong id);

        public virtual async Task<IDiscordMessage> EditMessageAsync(
			ulong channelId, ulong messageId, EditMessageArgs message)
        {
            return new DiscordMessage(
                await ApiClient.EditMessageAsync(channelId, messageId, message),
                this
            );
        }

        public virtual async Task<IDiscordTextChannel> CreateDMAsync(
            ulong userid)
        {
            var channel = await ApiClient.CreateDMChannelAsync(userid);

            return ResolveChannel(channel) as IDiscordTextChannel;
		}

		public virtual async Task<IDiscordRole> CreateRoleAsync(
            ulong guildId, 
            CreateRoleArgs args = null)
        {
            return new DiscordRole(
                await ApiClient.CreateGuildRoleAsync(guildId, args),
                this
            );
        }

        public virtual async Task<IDiscordRole> EditRoleAsync(
            ulong guildId, 
            DiscordRolePacket role)
        {
            return new DiscordRole(await ApiClient.EditRoleAsync(guildId, role), this);
        }

        public virtual async Task<IDiscordPresence> GetUserPresence(
            ulong userId, 
            ulong? guildId = null)
        {
            if (!guildId.HasValue)
            {
                throw new NotSupportedException("The default Discord Client cannot get the presence of the user without the guild ID. Use the cached client instead.");
            }

            // We have to get the guild because there is no API end-point for user presence.
            // This is a known issue: https://github.com/discordapp/discord-api-docs/issues/666

            var guild = await GetGuildPacketAsync(guildId.Value);
            var presence = guild.Presences.FirstOrDefault(p => p.User.Id == userId);

            return presence != null ? new DiscordPresence(presence) : null;
        }

		public virtual async Task<IDiscordRole> GetRoleAsync(
            ulong guildId, 
            ulong roleId)
        {
            return new DiscordRole(await GetRolePacketAsync(roleId, guildId), this);
        }

        public virtual async Task<IEnumerable<IDiscordRole>> GetRolesAsync(
            ulong guildId)
        {
            return (await GetRolePacketsAsync(guildId))
                .Select(x => new DiscordRole(x, this));
        }

        public virtual async Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
        {
            var channelPackets = await GetGuildChannelPacketsAsync(guildId);

            return channelPackets.Select(x => ResolveChannel(x) as IDiscordGuildChannel);
        }

		public virtual async Task<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
		{
			var channel = await GetChannelPacketAsync(id);

			return ResolveChannel(channel);
        }

        public virtual async Task<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) where T : class, IDiscordChannel
        {
            var channel = await GetChannelPacketAsync(id);

            return ResolveChannel(channel) as T;
        }

        public virtual async Task<IDiscordSelfUser> GetSelfAsync()
		{
			return new DiscordSelfUser(
				await GetCurrentUserPacketAsync(),
				this);
		}

		public virtual async Task<IDiscordGuild> GetGuildAsync(ulong id)
		{
			var packet = await GetGuildPacketAsync(id);

			return new DiscordGuild(
				packet,
				this
			);
		}

		public virtual async Task<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
		{
			return new DiscordGuildUser(
				await GetGuildMemberPacketAsync(id, guildId),
				this
			);
		}

        public async Task<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId)
        {
            return (await GetGuildMembersPacketAsync(guildId))
                .Select(x => new DiscordGuildUser(x, this));
        }

        public virtual async Task<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
		{
			var users = await ApiClient.GetReactionsAsync(channelId, messageId, emoji);

			if(users != null)
			{
				return users.Select(
					x => new DiscordUser(x, this)
				);
			}

			return new List<IDiscordUser>();
		}

		public virtual async Task<IDiscordUser> GetUserAsync(ulong id)
		{
			var packet = await GetUserPacketAsync(id);

			return new DiscordUser(
				packet,
				this
			);
		}

		public virtual async Task SetGameAsync(int shardId, DiscordStatus status)
		{
			await Gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
		}

		public virtual async Task<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null)
			=> new DiscordMessage(
				await ApiClient.SendFileAsync(channelId, stream, fileName, message),
				this
			);

		public virtual async Task<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message)
			=> new DiscordMessage(
				await ApiClient.SendMessageAsync(channelId, message),
				this
			);

        private IDiscordChannel ResolveChannel(DiscordChannelPacket packet)
		{
            switch (packet.Type)
			{
				case ChannelType.GUILDTEXT:
                case ChannelType.GUILDNEWS:
					return new DiscordGuildTextChannel(packet, this);

                case ChannelType.CATEGORY:
                case ChannelType.GUILDVOICE:
                    return new DiscordGuildChannel(packet, this);

                case ChannelType.DM:
                case ChannelType.GROUPDM:
                    return new DiscordTextChannel(packet, this);

                default:
                    throw new ArgumentOutOfRangeException();
            }
		}

        public virtual void Dispose()
        {
        }
    }
}