using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GatewayResumePacket
    {
        [DataMember(Name = "token", Order = 1)]
        public string Token { get; set; }

        [DataMember(Name = "session_id", Order = 2)]
        public string SessionId { get; set; }

        [DataMember(Name = "seq", Order = 3)]
        public int Sequence { get; set; }
    }
}