using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GuildMemberUpdateEventArgs
	{
		[JsonPropertyName("guild_id")]
		[DataMember(Name = "guild_id", Order = 1)]
		public ulong GuildId { get; set; }

        [JsonPropertyName("roles")]
		[DataMember(Name = "roles", Order = 2)]
		public ulong[] RoleIds { get; set; }

        [JsonPropertyName("user")]
		[DataMember(Name = "user", Order = 3)]
		public DiscordUserPacket User { get; set; }

        [JsonPropertyName("nick")]
		[DataMember(Name = "nick", Order = 4)]
		public string Nickname { get; set; }
    }
}