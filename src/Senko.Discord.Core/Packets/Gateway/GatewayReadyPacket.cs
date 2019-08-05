using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GatewayReadyPacket
	{
        [DataMember(Name = "v", Order = 1)]
        public int ProtocolVersion;

        [DataMember(Name = "user", Order = 2)]
        public DiscordUserPacket CurrentUser;

        [DataMember(Name = "private_channels", Order = 3)]
        public DiscordChannelPacket[] PrivateChannels;

        [DataMember(Name = "guilds", Order = 4)]
        public DiscordGuildPacket[] Guilds;

        [DataMember(Name = "session_id", Order = 5)]
        public string SessionId;

        [DataMember(Name = "_trace", Order = 6)]
        public string[] TraceGuilds;

        [DataMember(Name = "shard", Order = 7)]
        public int[] Shard;

		public int CurrentShard
			=> Shard[0];

		public int TotalShards
			=> Shard[1];
	}
}