using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GuildIdUserArgs
	{
		[JsonPropertyName("user")]
		[DataMember(Name = "user", Order = 1)]
		public DiscordUserPacket User { get; set; }

        [JsonPropertyName("guild_id")]
		[DataMember(Name = "guild_id", Order = 2)]
		public ulong GuildId { get; set; }
    }
}