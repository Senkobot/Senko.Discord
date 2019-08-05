using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class TypingStartEventArgs
	{
		[DataMember(Name ="channel_id", Order = 1)]
		public ulong ChannelId { get; set; }

		[DataMember(Name ="guild_id", Order = 2)]
		public ulong GuildId { get; set; }

		[DataMember(Name ="member", Order = 3)]
		public DiscordGuildMemberPacket Member { get; set; }
	}
}