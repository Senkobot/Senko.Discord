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

        public ValueTask StartAsync()
            => Gateway.StartAsync();

        public ValueTask StopAsync()
            => Gateway.StopAsync();

        protected abstract ValueTask<DiscordUserPacket> GetCurrentUserPacketAsync();

        protected abstract ValueTask<DiscordUserPacket> GetUserPacketAsync(ulong id);

        protected abstract ValueTask<DiscordGuildPacket> GetGuildPacketAsync(ulong id);

        protected abstract ValueTask<DiscordGuildMemberPacket> GetGuildMemberPacketAsync(ulong userId, ulong guildId);

        protected abstract ValueTask<DiscordGuildMemberPacket[]> GetGuildMembersPacketAsync(ulong guildId);

        protected abstract ValueTask<DiscordChannelPacket[]> GetGuildChannelPacketsAsync(ulong guildId);

        protected abstract ValueTask<DiscordRolePacket[]> GetRolePacketsAsync(ulong guildId);

        protected abstract ValueTask<DiscordRolePacket> GetRolePacketAsync(ulong roleId, ulong guildId);

        protected abstract ValueTask<DiscordChannelPacket> GetChannelPacketAsync(ulong id);

        public virtual async ValueTask<IDiscordMessage> EditMessageAsync(
			ulong channelId, ulong messageId, EditMessageArgs message)
        {
            return new DiscordMessage(
                await ApiClient.EditMessageAsync(channelId, messageId, message),
                this
            );
        }

        public ValueTask DeleteChannelAsync(ulong channelId)
        {
            return ApiClient.DeleteChannelAsync(channelId);
        }

        public ValueTask AddGuildBanAsync(ulong guildId, ulong userId, int pruneDays, string reason)
        {
            return ApiClient.AddGuildBanAsync(guildId, userId, pruneDays, reason);
        }

        public ValueTask RemoveGuildBanAsync(ulong guildId, ulong userId)
        {
            return ApiClient.RemoveGuildBanAsync(guildId, userId);
        }

        public ValueTask<int> GetPruneCountAsync(in ulong guildId, in int days)
        {
            return ApiClient.GetPruneCountAsync(guildId, days);
        }

        public ValueTask<int?> PruneGuildMembersAsync(ulong guildId, int days, bool computeCount)
        {
            return ApiClient.PruneGuildMembersAsync(guildId, days, computeCount);
        }

        public ValueTask DeleteMessagesAsync(ulong id, params ulong[] messageIds)
        {
            return ApiClient.DeleteMessagesAsync(id, messageIds);
        }

        public async ValueTask<IDiscordMessage> GetMessageAsync(ulong channelId, ulong messageId)
        {
            return new DiscordMessage(await ApiClient.GetMessageAsync(channelId, messageId), this);
        }

        public IAsyncEnumerable<IDiscordMessage> GetMessagesAsync(ulong channelId, int amount)
        {
            return ApiClient.GetMessagesAsync(channelId, amount).Select(p => new DiscordMessage(p, this));
        }

        public ValueTask TriggerTypingAsync(ulong channelId)
        {
            return ApiClient.TriggerTypingAsync(channelId);
        }

        public ValueTask AddGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            return ApiClient.AddGuildMemberRoleAsync(guildId, userId, roleId);
        }

        public ValueTask KickGuildMemberAsync(ulong guildId, ulong userId, string reason)
        {
            return ApiClient.KickGuildMemberAsync(guildId, userId, reason);
        }

        public ValueTask RemoveGuildMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            return ApiClient.RemoveGuildMemberRoleAsync(guildId, userId, roleId);
        }

        public ValueTask DeleteMessageAsync(ulong channelId, ulong packetId)
        {
            return ApiClient.DeleteMessageAsync(channelId, packetId);
        }

        public ValueTask DeleteReactionsAsync(ulong channelId, ulong messageId)
        {
            return ApiClient.DeleteReactionsAsync(channelId, messageId);
        }

        public ValueTask CreateReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return ApiClient.CreateReactionAsync(channelId, messageId, emoji);
        }

        public ValueTask DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
        {
            return ApiClient.DeleteReactionAsync(channelId, messageId, emoji);
        }

        public ValueTask DeleteReactionAsync(ulong channelId, ulong messageId, DiscordEmoji emoji, ulong userId)
        {
            return ApiClient.DeleteReactionAsync(channelId, messageId, emoji, userId);
        }

        public ValueTask ModifySelfAsync(UserModifyArgs args)
        {
            return ApiClient.ModifySelfAsync(args);
        }

        public virtual async ValueTask<IDiscordTextChannel> CreateDMAsync(
            ulong userid)
        {
            var channel = await ApiClient.CreateDMChannelAsync(userid);

            return this.GetChannelFromPacket(channel) as IDiscordTextChannel;
		}

		public virtual async ValueTask<IDiscordRole> CreateRoleAsync(
            ulong guildId, 
            CreateRoleArgs args = null)
        {
            return new DiscordRole(
                await ApiClient.CreateGuildRoleAsync(guildId, args),
                this
            );
        }

        public virtual async ValueTask<IDiscordRole> EditRoleAsync(
            ulong guildId, 
            DiscordRolePacket role)
        {
            return new DiscordRole(await ApiClient.EditRoleAsync(guildId, role), this);
        }

        public virtual async ValueTask<IDiscordPresence> GetUserPresence(
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

		public virtual async ValueTask<IDiscordRole> GetRoleAsync(
            ulong guildId, 
            ulong roleId)
        {
            return new DiscordRole(await GetRolePacketAsync(roleId, guildId), this);
        }

        public virtual async ValueTask<IEnumerable<IDiscordRole>> GetRolesAsync(
            ulong guildId)
        {
            return (await GetRolePacketsAsync(guildId))
                .Select(x => new DiscordRole(x, this));
        }

        public virtual async ValueTask<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync(ulong guildId)
        {
            var channelPackets = await GetGuildChannelPacketsAsync(guildId);

            return channelPackets
                .Select(this.GetChannelFromPacket)
                .OfType<IDiscordGuildChannel>();
        }

		public virtual async ValueTask<IDiscordChannel> GetChannelAsync(ulong id, ulong? guildId = null)
		{
			var channel = await GetChannelPacketAsync(id);

			return this.GetChannelFromPacket(channel);
        }

        public virtual async ValueTask<T> GetChannelAsync<T>(ulong id, ulong? guildId = null) where T : class, IDiscordChannel
        {
            var channel = await GetChannelPacketAsync(id);

            return this.GetChannelFromPacket(channel) as T;
        }

        public virtual async ValueTask<IDiscordSelfUser> GetSelfAsync()
		{
			return new DiscordSelfUser(
				await GetCurrentUserPacketAsync(),
				this);
		}

		public virtual async ValueTask<IDiscordGuild> GetGuildAsync(ulong id)
		{
			var packet = await GetGuildPacketAsync(id);

			return new DiscordGuild(
				packet,
				this
			);
		}

		public virtual async ValueTask<IDiscordGuildUser> GetGuildUserAsync(ulong id, ulong guildId)
		{
			return new DiscordGuildUser(
				await GetGuildMemberPacketAsync(id, guildId),
				this
			);
		}

        public async ValueTask<IEnumerable<IDiscordGuildUser>> GetGuildUsersAsync(ulong guildId)
        {
            return (await GetGuildMembersPacketAsync(guildId))
                .Select(x => new DiscordGuildUser(x, this));
        }

        public virtual async ValueTask<IEnumerable<IDiscordUser>> GetReactionsAsync(ulong channelId, ulong messageId, DiscordEmoji emoji)
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

		public virtual async ValueTask<IDiscordUser> GetUserAsync(ulong id)
		{
			var packet = await GetUserPacketAsync(id);

			return new DiscordUser(
				packet,
				this
			);
		}

		public virtual async ValueTask SetGameAsync(int shardId, DiscordStatus status)
		{
			await Gateway.SendAsync(shardId, GatewayOpcode.StatusUpdate, status);
		}

		public virtual async ValueTask<IDiscordMessage> SendFileAsync(ulong channelId, Stream stream, string fileName, MessageArgs message = null)
			=> new DiscordMessage(
				await ApiClient.SendFileAsync(channelId, stream, fileName, message),
				this
			);

		public virtual async ValueTask<IDiscordMessage> SendMessageAsync(ulong channelId, MessageArgs message)
			=> new DiscordMessage(
				await ApiClient.SendMessageAsync(channelId, message),
				this
			);

        public virtual void Dispose()
        {
        }
    }
}