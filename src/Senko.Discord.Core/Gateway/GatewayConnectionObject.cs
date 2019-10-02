using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Gateway
{
	public class GatewayConnectionPacket
	{
		[JsonPropertyName("url")]
		[DataMember(Name = "url")]
		public string Url { get; set; }

        [JsonPropertyName("shards")]
		[DataMember(Name = "shards")]
		public int ShardCount { get; set; }

        [JsonPropertyName("session_start_limit")]
		[DataMember(Name = "session_start_limit")]
		public GatewaySessionLimitsPacket SessionLimit { get; set; }
    }

	public class GatewaySessionLimitsPacket
	{
		[JsonPropertyName("total")]
		[DataMember(Name = "total")]
		public int Total { get; set; }

        [JsonPropertyName("remaining")]
		[DataMember(Name = "remaining")]
		public int Remaining { get; set; }

        [JsonPropertyName("reset_after")]
		[DataMember(Name = "reset_after")]
		public int ResetAfter { get; set; }
    }
}