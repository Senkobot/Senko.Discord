using System;
using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
	public class GatewayIdentifyPacket
	{
        [DataMember(Name = "token", Order = 1)]
		public string Token;

        [DataMember(Name = "properties", Order = 2)]
        public GatewayIdentifyConnectionProperties ConnectionProperties = new GatewayIdentifyConnectionProperties();

        [DataMember(Name = "compress", Order = 3)]
        public bool Compressed;

        [DataMember(Name = "large_threshold", Order = 4)]
        public int LargeThreshold;

        [DataMember(Name = "presence", Order = 5)]
        public DiscordStatus Presence;

        [DataMember(Name = "shard", Order = 6)]
        public int[] Shard;
	}

    [DataContract]
    public class GatewayIdentifyConnectionProperties
	{
        [DataMember(Name = "$os")]
        public string OperatingSystem = Environment.OSVersion.ToString();

        [DataMember(Name = "$browser")]
        public string Browser = "Senko.Discord";

        [DataMember(Name = "$device")]
        public string Device = "Senko.Discord";
	}
}