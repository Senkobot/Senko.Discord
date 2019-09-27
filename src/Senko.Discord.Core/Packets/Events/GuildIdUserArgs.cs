using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GuildIdUserArgs
	{
		[DataMember(Name ="user", Order = 1)]
		public DiscordUserPacket User;

		[DataMember(Name ="guild_id", Order = 2)]
		public ulong GuildId;
	}
}