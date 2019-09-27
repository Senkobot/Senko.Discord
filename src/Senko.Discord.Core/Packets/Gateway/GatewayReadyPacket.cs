using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GatewayReadyPacket
	{
        [JsonPropertyName("v")]
        [DataMember(Name = "v", Order = 1)]
        public int ProtocolVersion { get; set; }

        [JsonPropertyName("user")]
        [DataMember(Name = "user", Order = 2)]
        public DiscordUserPacket CurrentUser { get; set; }

        [JsonPropertyName("private_channels")]
        [DataMember(Name = "private_channels", Order = 3)]
        public DiscordChannelPacket[] PrivateChannels { get; set; }

        [JsonPropertyName("guilds")]
        [DataMember(Name = "guilds", Order = 4)]
        public DiscordGuildPacket[] Guilds { get; set; }

        [JsonPropertyName("session_id")]
        [DataMember(Name = "session_id", Order = 5)]
        public string SessionId { get; set; }

        [JsonPropertyName("_trace")]
        [DataMember(Name = "_trace", Order = 6)]
        public string[] TraceGuilds { get; set; }

        [JsonPropertyName("shard")]
        [DataMember(Name = "shard", Order = 7)]
        public int[] Shard { get; set; }

        public int CurrentShard
			=> Shard[0];

		public int TotalShards
			=> Shard[1];
	}
}