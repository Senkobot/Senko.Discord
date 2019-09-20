using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class MessageReactionArgs
    {
        [DataMember(Name = "user_id", Order = 1)]
        public ulong UserId { get; set; }

        [DataMember(Name = "message_id", Order = 2)]
        public ulong MessageId { get; set; }

        [DataMember(Name = "emoji", Order = 3)]
        public DiscordEmoji Emoji { get; set; }

        [DataMember(Name = "channel_id", Order = 4)]
        public ulong ChannelId { get; set; }

        [DataMember(Name = "guild_id", Order = 5)]
        public ulong? GuildId { get; set; }
    }
}
