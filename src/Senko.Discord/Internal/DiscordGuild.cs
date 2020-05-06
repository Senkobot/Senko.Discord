using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public class DiscordGuild : IDiscordGuild
	{
		private readonly DiscordGuildPacket _packet;
		private readonly IDiscordClient _client;

		public DiscordGuild(DiscordGuildPacket packet, IDiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public string Name
			=> _packet.Name;

		public ulong Id
			=> _packet.Id;

		public string IconUrl
			=> DiscordUtils.GetAvatarUrl(Id, _packet.Icon);

		public ulong OwnerId
			=> _packet.OwnerId;

		public int MemberCount
			=> _packet.MemberCount ?? 0;

		public GuildPermission Permissions
			=> (GuildPermission)_packet.Permissions.GetValueOrDefault(0);

        public int PremiumSubscriberCount 
            => _packet.PremiumSubscriberCount.GetValueOrDefault(0);

        public int PremiumTier 
            => _packet.PremiumTier.GetValueOrDefault(0);

        public async ValueTask AddBanAsync(IDiscordGuildUser user, int pruneDays = 7, string reason = null)
		{
			await _client.AddGuildBanAsync(Id, user.Id, pruneDays, reason);
		}

		public ValueTask<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleParams = null)
        {
            return _client.CreateRoleAsync(Id, roleParams);
        }

        public ValueTask<IDiscordGuildChannel> GetChannelAsync(ulong id)
        {
            return _client.GetChannelAsync<IDiscordGuildChannel>(id, Id);
        }

        public async ValueTask<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync()
		{
			return await _client.GetChannelsAsync(Id);
		}

		public ValueTask<IDiscordChannel> GetDefaultChannelAsync()
		{
			if (!_packet.SystemChannelId.HasValue)
			{
				return default;
			}

			return _client.GetChannelAsync(_packet.SystemChannelId.Value, Id);
		}

		public ValueTask<IDiscordGuildUser> GetMemberAsync(ulong id)
		{
			return _client.GetGuildUserAsync(id, Id);
		}

		public ValueTask<IEnumerable<IDiscordGuildUser>> GetMembersAsync()
        {
            return _client.GetGuildUsersAsync(Id);
		}

		public ValueTask<IDiscordGuildUser> GetOwnerAsync()
        {
            return GetMemberAsync(OwnerId);
        }

        public async ValueTask<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
		{
			if (user.Id == OwnerId)
			{
				return GuildPermission.All;
			}

			GuildPermission permissions = Permissions;

			if (permissions.HasFlag(GuildPermission.Administrator))
			{
				return GuildPermission.All;
			}

			IDiscordRole everyoneRole = await GetRoleAsync(Id);
			permissions = everyoneRole.Permissions;

			if (user.RoleIds != null)
			{
                var roles = await GetRolesAsync();
				foreach (IDiscordRole role in roles.Where(x => user.RoleIds.Contains(x.Id)))
				{
					permissions |= role.Permissions;
				}
			}

			if (permissions.HasFlag(GuildPermission.Administrator))
			{
				return GuildPermission.All;
			}
			return permissions;
		}

        public ValueTask<int> GetPruneCountAsync(int days)
        {
            return _client.GetPruneCountAsync(Id, days);
        }

        public ValueTask<IDiscordRole> GetRoleAsync(ulong id)
        {
            return _client.GetRoleAsync(Id, id);
        }

        public ValueTask<IEnumerable<IDiscordRole>> GetRolesAsync()
        {
            return _client.GetRolesAsync(Id);
        }

        public async ValueTask<IDiscordGuildUser> GetSelfAsync()
		{
            IDiscordUser user = await _client.GetSelfAsync();

            if (user == null)
            {
                throw new InvalidOperationException("Could not find self user");
            }

			return await GetMemberAsync(user.Id);
		}

        public async ValueTask<int?> PruneMembersAsync(int days, bool computeCount = false)
        {
            // NOTE: It is not recommended to compute these counts for large guilds.
            if(computeCount && MemberCount > 1000)
            {
                computeCount = false;
            }

            return await _client.PruneGuildMembersAsync(Id, days, computeCount);
        }

        public async ValueTask RemoveBanAsync(IDiscordGuildUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _client.RemoveGuildBanAsync(Id, user.Id);
        }
    }
}