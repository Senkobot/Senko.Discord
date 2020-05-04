using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Senko.Discord.Gateway;

namespace Senko.Discord.Packets
{
    [DataContract]
	public class GatewayIdentifyPacket
	{
        [JsonPropertyName("token")]
        [DataMember(Name = "token", Order = 1)]
		public string Token { get; set; }

        [JsonPropertyName("properties")]
        [DataMember(Name = "properties", Order = 2)]
        public GatewayIdentifyConnectionProperties ConnectionProperties { get; set; } = new GatewayIdentifyConnectionProperties();

        [JsonPropertyName("compress")]
        [DataMember(Name = "compress", Order = 3)]
        public bool Compressed { get; set; }

        [JsonPropertyName("large_threshold")]
        [DataMember(Name = "large_threshold", Order = 4)]
        public int LargeThreshold { get; set; }

        [JsonPropertyName("presence")]
        [DataMember(Name = "presence", Order = 5)]
        public DiscordStatus Presence { get; set; }

        [JsonPropertyName("shard")]
        [DataMember(Name = "shard", Order = 6)]
        public int[] Shard { get; set; }

        [JsonPropertyName("intents")]
        [DataMember(Name = "intents", Order = 7)]
        public GatewayIntent? Intents { get; set; }
    }

    [DataContract]
    public class GatewayIdentifyConnectionProperties
	{
        [JsonPropertyName("$os")]
        [DataMember(Name = "$os")]
        public string OperatingSystem { get; set; } = Environment.OSVersion.ToString();

        [JsonPropertyName("$browser")]
        [DataMember(Name = "$browser")]
        public string Browser { get; set; } = "Senko.Discord";

        [JsonPropertyName("$device")]
        [DataMember(Name = "$device")]
        public string Device { get; set; } = "Senko.Discord";
	}
}