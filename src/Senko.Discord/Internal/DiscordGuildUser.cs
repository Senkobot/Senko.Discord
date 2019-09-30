using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord.Internal
{
	public class DiscordGuildUser : DiscordUser, IDiscordGuildUser
	{
		private readonly DiscordGuildMemberPacket _packet;

		public DiscordGuildUser(DiscordGuildMemberPacket packet, IDiscordClient client)
            : base(packet.User, client)
		{
			_packet = packet;
        }

		public string Nickname
			=> _packet.Nickname;

		public IReadOnlyCollection<ulong> RoleIds
			=> _packet.Roles;

		public ulong GuildId
			=> _packet.GuildId;

		public DateTimeOffset JoinedAt
			=> _packet.JoinedAt;

        public DateTimeOffset? PremiumSince
            => _packet.PremiumSince;

        public ValueTask AddRoleAsync(IDiscordRole role)
		{
			return Client.AddGuildMemberRoleAsync(GuildId, Id, role.Id);
		}

		public ValueTask<IDiscordGuild> GetGuildAsync()
        {
            return Client.GetGuildAsync(_packet.GuildId);
        }

        public async ValueTask KickAsync(string reason = null)
		{
			await Client.KickGuildMemberAsync(GuildId, Id, reason);
		}

		public async ValueTask RemoveRoleAsync(IDiscordRole role)
		{
            if(role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

			await Client.RemoveGuildMemberRoleAsync(GuildId, Id, role.Id);
		}

		public async ValueTask<bool> HasPermissionsAsync(GuildPermission permissions)
		{
            var guild = await GetGuildAsync();
			GuildPermission p = await guild.GetPermissionsAsync(this);
			return p.HasFlag(permissions);
		}

		public async ValueTask<int> GetHierarchyAsync()
        {
            var guild = await GetGuildAsync();
            return (await guild.GetRolesAsync())
				.Where(x => RoleIds.Contains(x.Id))
				.Max(x => x.Position);
		}
	}
}