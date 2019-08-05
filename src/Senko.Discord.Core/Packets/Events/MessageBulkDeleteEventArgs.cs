
using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class MessageBulkDeleteEventArgs
	{
		[DataMember(Name ="guild_id", Order = 1)]
		public ulong GuildId { get; set; }

		[DataMember(Name ="channel_id")]
		public ulong ChannelId { get; set; }

		[DataMember(Name ="ids")]
		public ulong[] MessagesDeleted { get; set; }
	}
}