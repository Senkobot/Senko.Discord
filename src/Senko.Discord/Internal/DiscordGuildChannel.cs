using System.Linq;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public class DiscordGuildChannel : DiscordChannel, IDiscordGuildChannel
	{
		public DiscordGuildChannel(DiscordChannelPacket packet, IDiscordClient client, ulong guildId)
			: base(packet, client)
        {
            GuildId = guildId;
        }

		public new ulong GuildId { get; }

		public ChannelType Type
			=> _packet.Type;

		public ValueTask<IDiscordGuild> GetGuildAsync()
        {
            return _client.GetGuildAsync(GuildId);
        }

        public async ValueTask<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
		{
			IDiscordGuild guild = await GetGuildAsync();

			GuildPermission permissions = await guild.GetPermissionsAsync(user);

			if (permissions.HasFlag(GuildPermission.Administrator))
			{
				return GuildPermission.All;
			}

			if (_packet.PermissionOverwrites != null)
			{
				PermissionOverwrite overwriteEveryone = _packet.PermissionOverwrites
					.FirstOrDefault(x => x.Id == GuildId) ?? null;

				if (overwriteEveryone != null)
				{
					permissions &= ~overwriteEveryone.DeniedPermissions;
					permissions |= overwriteEveryone.AllowedPermissions;
				}

				PermissionOverwrite overwrites = new PermissionOverwrite();

				if (user.RoleIds != null)
				{
					foreach (ulong roleId in user.RoleIds)
					{
						PermissionOverwrite roleOverwrites = _packet.PermissionOverwrites.FirstOrDefault(x => x.Id == roleId);

						if (roleOverwrites != null)
						{
							overwrites.AllowedPermissions |= roleOverwrites.AllowedPermissions;
							overwrites.DeniedPermissions &= roleOverwrites.DeniedPermissions;
						}
					}
				}

				permissions &= ~overwrites.DeniedPermissions;
				permissions |= overwrites.AllowedPermissions;

				PermissionOverwrite userOverwrite = _packet.PermissionOverwrites.FirstOrDefault(x => x.Id == user.Id);

				if (userOverwrite != null)
				{
					permissions &= ~userOverwrite.DeniedPermissions;
					permissions |= userOverwrite.AllowedPermissions;
				}
			}

			return permissions;
		}

		public ValueTask<IDiscordGuildUser> GetUserAsync(ulong id)
        {
            return _client.GetGuildUserAsync(id, GuildId);
        }
    }
}