using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GuildEmojisUpdateEventArgs
	{
		[JsonPropertyName("guild_id")]
		[DataMember(Name = "guild_id", Order = 1)]
		public ulong GuildId { get; set; }

        [JsonPropertyName("emojis")]
		[DataMember(Name = "emojis", Order = 2)]
		public DiscordEmoji[] Emojis { get; set; }
    }
}