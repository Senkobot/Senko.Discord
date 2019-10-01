using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class TypingStartEventArgs
	{
		[JsonPropertyName("channel_id")]
		[DataMember(Name = "channel_id", Order = 1)]
		public ulong ChannelId { get; set; }

		[JsonPropertyName("guild_id")]
		[DataMember(Name = "guild_id", Order = 2)]
		public ulong GuildId { get; set; }

		[JsonPropertyName("member")]
		[DataMember(Name = "member", Order = 3)]
		public DiscordGuildMemberPacket Member { get; set; }
	}
}