﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class MessageDeleteArgs
	{
		[JsonPropertyName("id")]
		[DataMember(Name = "id", Order = 1)]
		public ulong MessageId { get; set; }

		[JsonPropertyName("channel_id")]
		[DataMember(Name = "channel_id", Order = 2)]
		public ulong ChannelId { get; set; }
	}
}