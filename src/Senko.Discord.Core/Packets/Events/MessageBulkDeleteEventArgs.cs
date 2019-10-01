
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class MessageBulkDeleteEventArgs
	{
		[JsonPropertyName("guild_id")]
		[DataMember(Name = "guild_id", Order = 1)]
		public ulong GuildId { get; set; }

		[JsonPropertyName("channel_id")]
		[DataMember(Name = "channel_id")]
		public ulong ChannelId { get; set; }

		[JsonPropertyName("ids")]
		[DataMember(Name = "ids")]
		public ulong[] MessagesDeleted { get; set; }
	}
}