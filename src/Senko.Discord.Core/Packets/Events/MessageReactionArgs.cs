using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class MessageReactionArgs
    {
        [JsonPropertyName("user_id")]
        [DataMember(Name = "user_id", Order = 1)]
        public ulong UserId { get; set; }

        [JsonPropertyName("message_id")]
        [DataMember(Name = "message_id", Order = 2)]
        public ulong MessageId { get; set; }

        [JsonPropertyName("emoji")]
        [DataMember(Name = "emoji", Order = 3)]
        public DiscordEmoji Emoji { get; set; }

        [JsonPropertyName("channel_id")]
        [DataMember(Name = "channel_id", Order = 4)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        [DataMember(Name = "guild_id", Order = 5)]
        public ulong? GuildId { get; set; }
    }
}
