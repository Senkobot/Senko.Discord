using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
	[Serializable]
    [DataContract]
    public class DiscordChannelPacket : ISnowflake
	{
        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
		public ulong Id { get; set; }

        [JsonPropertyName("type")]
        [DataMember(Name = "type", Order = 2)]
		public ChannelType Type { get; set; }

        [JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 3)]
		public string Name { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 4)]
		public ulong? GuildId { get; set; }

        [JsonPropertyName("position")]
        [DataMember(Name = "position", Order = 5)]
		public int? Position { get; set; }

        [JsonPropertyName("permission_overwrites")]
        [DataMember(Name = "permission_overwrites", Order = 6)]
		public PermissionOverwrite[] PermissionOverwrites { get; set; }

        [JsonPropertyName("parent_id")]
        [DataMember(Name = "parent_id", Order = 7)]
		public ulong? ParentId { get; set; }

        [JsonPropertyName("nsfw")]
        [DataMember(Name = "nsfw", Order = 8)]
		public bool? IsNsfw { get; set; }

        [JsonPropertyName("topic")]
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