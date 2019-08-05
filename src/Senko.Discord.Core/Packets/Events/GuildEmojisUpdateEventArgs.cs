using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GuildEmojisUpdateEventArgs
	{
		[DataMember(Name ="guild_id", Order = 1)]
		public ulong GuildId;

		[DataMember(Name ="emojis", Order = 2)]
		public DiscordEmoji[] Emojis;
	}
}