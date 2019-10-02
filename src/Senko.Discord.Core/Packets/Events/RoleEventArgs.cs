using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class RoleEventArgs
	{
		[JsonPropertyName("guild_id")]
		[DataMember(Name = "guild_id", Order = 1)]
		public ulong GuildId { get; set; }

		[JsonPropertyName("role")]
		[DataMember(Name = "role", Order = 2)]
		public DiscordRolePacket Role { get; set; }
	}
}