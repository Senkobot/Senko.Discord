using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordGuildMemberPacket : ISnowflake
	{
        [DataMember(Name = "user", Order = 1)]
		public DiscordUserPacket User { get; set; }

        [DataMember(Name = "guild_id", Order = 2)]
		public ulong GuildId { get; set; }

        [DataMember(Name = "nick", Order = 3)]
		public string Nickname { get; set; }

        [DataMember(Name = "roles", Order = 4)]
		public List<ulong> Roles { get; set; } = new List<ulong>();

        [DataMember(Name = "joined_at", Order = 5)]
		public DateTimeOffset JoinedAt { get; set; }
        
        [DataMember(Name = "deaf", Order = 6)]
		public bool Deafened { get; set; }

        [DataMember(Name = "mute", Order = 7)]
		public bool Muted { get; set; }

        [DataMember(Name = "premium_since", Order = 8)]
        public DateTimeOffset? PremiumSince { get; set; }

        ulong ISnowflake.Id => User.Id;
    }
}