using System;
using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
	[Serializable]
    [DataContract]
    public class DiscordChannelPacket : ISnowflake
	{
        [DataMember(Name = "id", Order = 1)]
		public ulong Id { get; set; }

        [DataMember(Name = "type", Order = 2)]
		public ChannelType Type { get; set; }

        [DataMember(Name = "name", Order = 3)]
		public string Name { get; set; }

        [DataMember(Name = "guild_id", Order = 4)]
		public ulong? GuildId { get; set; }

        [DataMember(Name = "position", Order = 5)]
		public int? Position { get; set; }

        [DataMember(Name = "permission_overwrites", Order = 6)]
		public PermissionOverwrite[] PermissionOverwrites { get; set; }

        [DataMember(Name = "parent_id", Order = 7)]
		public ulong? ParentId { get; set; }

        [DataMember(Name = "nsfw", Order = 8)]
		public bool? IsNsfw { get; set; }

        [DataMember(Name = "topic", Order = 9)]
		public string Topic { get; set; }
	}

	public enum ChannelType
	{
		GUILDTEXT = 0,
		DM = 1,
		GUILDVOICE = 2,
		GROUPDM = 3,
		CATEGORY = 4,
        GUILDNEWS = 5,
        GUILDSTORE = 6
	}
}