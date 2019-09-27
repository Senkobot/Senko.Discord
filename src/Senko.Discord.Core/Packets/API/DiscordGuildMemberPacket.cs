using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordGuildMemberPacket : ISnowflake
	{
        [JsonPropertyName("user")]
        [DataMember(Name = "user", Order = 1)]
		public DiscordUserPacket User { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 2)]
		public ulong GuildId { get; set; }

        [JsonPropertyName("nick")]
        [DataMember(Name = "nick", Order = 3)]
		public string Nickname { get; set; }

        [JsonPropertyName("roles")]
        [DataMember(Name = "roles", Order = 4)]
		public List<ulong> Roles { get; set; } = new List<ulong>();

        [JsonPropertyName("joined_at")]
        [DataMember(Name = "joined_at", Order = 5)]
		public DateTimeOffset JoinedAt { get; set; }
        
        [JsonPropertyName("deaf")]
        [DataMember(Name = "deaf", Order = 6)]
		public bool Deafened { get; set; }

        [JsonPropertyName("mute")]
        [DataMember(Name = "mute", Order = 7)]
		public bool Muted { get; set; }

        [JsonPropertyName("premium_since")]
        [DataMember(Name = "premium_since", Order = 8)]
        public DateTimeOffset? PremiumSince { get; set; }

        ulong ISnowflake.Id => User.Id;
    }
}