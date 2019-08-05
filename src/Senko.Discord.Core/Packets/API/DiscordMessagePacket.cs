using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
	[Serializable]
    [DataContract]
    public class DiscordMessagePacket
	{
		[DataMember(Name = "id", Order = 1)]
		public ulong Id { get; set; }

        [DataMember(Name = "channel_id", Order = 2)]
        public ulong ChannelId { get; set; }

        [DataMember(Name = "guild_id", Order = 3)]
        public ulong? GuildId { get; set; }

        [DataMember(Name = "author", Order = 4)]
        public DiscordUserPacket Author { get; set; }

        [DataMember(Name = "member", Order = 5)]
        public DiscordGuildMemberPacket Member { get; set; }

        [DataMember(Name = "type", Order = 6)]
		public DiscordMessageType Type { get; set; }

		[DataMember(Name = "content", Order = 7)]
		public string Content { get; set; }

		[DataMember(Name = "timestamp", Order = 8)]
		public DateTimeOffset Timestamp { get; set; }

		[DataMember(Name = "tts", Order = 9)]
		public bool IsTTS { get; set; }

		[DataMember(Name = "mention_everyone", Order = 10)]
		public bool MentionsEveryone { get; set; }

		[DataMember(Name = "mentions", Order = 11)]
		public List<DiscordUserPacket> Mentions { get; set; }

        [DataMember(Name = "attachments", Order = 12)]
        public List<DiscordAttachmentPacket> Attachments { get; set; }
	}

    /// <summary>
    /// Type of message received from the Discord API.
    /// </summary>
	public enum DiscordMessageType
	{
        /// <summary>
        /// Default text message from a user.
        /// </summary>
        DEFAULT = 0,
        /// <summary>
        /// Recipent added to a DM group.
        /// </summary>
        RECIPIENT_ADD,
        /// <summary>
        /// Recipent removed from a DM group.
        /// </summary>
        RECIPIENT_REMOVE,
        /// <summary>
        /// Receiving a voice call request from another user.
        /// </summary>
        CALL,
        /// <summary>
        /// DM channel name change event
        /// </summary>
        CHANNEL_NAME_CHANGE,
        /// <summary>
        /// DM channel icon change event
        /// </summary>
        CHANNEL_ICON_CHANGE,
        /// <summary>
        /// Message has been pinned in a channel
        /// </summary>
        CHANNEL_PINNED_MESSAGE,
        /// <summary>
        /// A guild member joined, and the discord guild has default join events turned on.
        /// </summary>
        GUILD_MEMBER_JOIN,
        USER_PREMIUM_GUILD_SUBSCRIPTION,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_1,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_2,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_3
    }
}