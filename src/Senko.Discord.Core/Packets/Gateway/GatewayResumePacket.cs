using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GatewayResumePacket
    {
        [JsonPropertyName("token")]
        [DataMember(Name = "token", Order = 1)]
        public string Token { get; set; }

        [JsonPropertyName("session_id")]
        [DataMember(Name = "session_id", Order = 2)]
        public string SessionId { get; set; }

        [JsonPropertyName("seq")]
        [DataMember(Name = "seq", Order = 3)]
        public int Sequence { get; set; }
    }
}