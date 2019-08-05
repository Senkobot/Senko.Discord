using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
	public class GatewayHelloPacket
	{
        [DataMember(Name = "heartbeat_interval", Order = 1)]
        public int HeartbeatInterval;

        [DataMember(Name = "_trace", Order = 2)]
        public string[] TraceServers;
	}
}