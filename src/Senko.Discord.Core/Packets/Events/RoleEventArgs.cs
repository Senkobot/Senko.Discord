using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class RoleEventArgs
	{
		[DataMember(Name ="guild_id", Order = 1)]
		public ulong GuildId { get; set; }

		[DataMember(Name ="role", Order = 2)]
		public DiscordRolePacket Role { get; set; }
	}
}