using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordRolePacket : ISnowflake
	{
		[DataMember(Name = "id", Order = 1)]
		public ulong Id { get; set; }

		[DataMember(Name = "name", Order = 2)]
		public string Name { get; set; }

        [DataMember(Name = "color", Order = 3)]
		public int Color { get; set; }

        [DataMember(Name = "hoist", Order = 4)]
		public bool IsHoisted { get; set; }

        [DataMember(Name = "position", Order = 5)]
		public int Position { get; set; }

        [DataMember(Name = "permissions", Order = 6)]
		public int Permissions { get; set; }

        [DataMember(Name = "managed", Order = 7)]
		public bool Managed { get; set; }

        [DataMember(Name = "mentionable", Order = 8)]
		public bool Mentionable { get; set; }
    }
}