using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
	public class GatewayHelloPacket
	{
        [JsonPropertyName("heartbeat_interval")]
        [DataMember(Name = "heartbeat_interval", Order = 1)]
        public int HeartbeatInterval { get; set; }

        [JsonPropertyName("_trace")]
        [DataMember(Name = "_trace", Order = 2)]
        public string[] TraceServers { get; set; }
    }
}