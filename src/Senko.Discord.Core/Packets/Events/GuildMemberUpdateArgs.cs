using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class ModifyGuildMemberArgs
	{
		[JsonPropertyName("nick")]
		[DataMember(Name = "nick", Order = 1)]
		public string Nickname { get; set; }

        [JsonPropertyName("roles")]
		[DataMember(Name = "roles", Order = 2)]
		public ulong[] RoleIds { get; set; }

        [JsonPropertyName("mute")]
		[DataMember(Name = "mute", Order = 3)]
		public bool? Muted { get; set; }

        [JsonPropertyName("deaf")]
		[DataMember(Name = "deaf", Order = 4)]
		public bool? Deafened { get; set; }

        [JsonPropertyName("channel_id")]
		[DataMember(Name = "channel_id", Order = 5)]
		public ulong? MoveToChannelId { get; set; }
    }
}