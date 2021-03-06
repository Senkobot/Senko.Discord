﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordGuildUnavailablePacket
	{
		[JsonPropertyName("id")]
		[DataMember(Name = "id", Order = 1)]
		public ulong GuildId { get; set; }

        [JsonPropertyName("unavailable")]
		[DataMember(Name = "unavailable", Order = 2)]
		public bool? IsUnavailable { get; set; }

		/// <summary>
		/// A converter method to avoid protocol buffer serialization complexion
		/// </summary>
		/// <returns>A converted DiscordGuildPacket</returns>
		public DiscordGuildPacket ToGuildPacket()
		{
			return new DiscordGuildPacket
			{
				Id = GuildId,
				Unavailable = IsUnavailable
			};
		}
	}
}