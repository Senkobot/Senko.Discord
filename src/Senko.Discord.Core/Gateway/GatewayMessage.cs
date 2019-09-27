using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Gateway
{
    public interface IGatewayMessage
    {
        GatewayOpcode OpCode { get; }

        int? SequenceNumber { get; }

        string EventName { get; }

        object Data { get; }
    }

    [DataContract]
    public struct GatewayMessage : IGatewayMessage
    {
        [JsonPropertyName("op")]
        [DataMember(Name = "op", Order = 1)]
        public GatewayOpcode OpCode { get; set; }

        GatewayOpcode IGatewayMessage.OpCode => OpCode;

        int? IGatewayMessage.SequenceNumber => null;

        string IGatewayMessage.EventName => null;

        object IGatewayMessage.Data => null;
    }

    [DataContract]
	public struct GatewayMessage<T> : IGatewayMessage
    {
        [JsonPropertyName("op")]
        [DataMember(Name = "op", Order = 1)]
        public GatewayOpcode OpCode { get; set; }

        [JsonPropertyName("t")]
        [DataMember(Name = "t", Order = 2)]
        public string EventName { get; set; }

        [JsonPropertyName("s")]
        [DataMember(Name = "s", Order = 3)]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("d")]
        [DataMember(Name = "d", Order = 4)]
        public T Data { get; set; }

        GatewayOpcode IGatewayMessage.OpCode => OpCode;

        int? IGatewayMessage.SequenceNumber => SequenceNumber;

        string IGatewayMessage.EventName => EventName;

        object IGatewayMessage.Data => Data;
    }
}