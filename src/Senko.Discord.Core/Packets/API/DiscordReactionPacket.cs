using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordReactionPacket
	{
		[JsonPropertyName("count")]
		[DataMember(Name = "count", Order = 1)]
		public int Count { get; set; }

		[JsonPropertyName("me")]
		[DataMember(Name = "me", Order = 2)]
		public bool Me { get; set; }

		[JsonPropertyName("emoji")]
		[DataMember(Name = "emoji", Order = 3)]
		public DiscordEmoji Emoji { get; set; }
	}
}