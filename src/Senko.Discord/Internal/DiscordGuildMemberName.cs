using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Senko.Discord.Helpers;
using Senko.Discord.Packets;

namespace Senko.Discord.Internal
{
    [DataContract]
    public class DiscordGuildMemberName : IDiscordGuildMemberName
    {
        public DiscordGuildMemberName()
        {
        }

        public DiscordGuildMemberName(DiscordGuildMemberPacket packet)
        {
            Id = packet.User.Id;
            Username = packet.User.Username;
            NormalizedUsername = StringHelper.Normalize(packet.User.Username);
            Nickname = packet.Nickname;
            NormalizedNickname = StringHelper.Normalize(packet.Nickname);
        }

        [JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }
		
        [JsonPropertyName("username")]
        [DataMember(Name = "username", Order = 2)]
        public string Username { get; set; }
		
        [JsonPropertyName("normalized_username")]
        [DataMember(Name = "normalized_username", Order = 3)]
        public string NormalizedUsername { get; set; }
		
        [JsonPropertyName("nickname")]
        [DataMember(Name = "nickname", Order = 4)]
        public string Nickname { get; set; }
		
        [JsonPropertyName("normalized_nickname")]
        [DataMember(Name = "normalized_nickname", Order = 5)]
        public string NormalizedNickname { get; set; }

        public bool Matches(string name)
        {
            return Nickname != null
                   && (Nickname.Contains(name, StringComparison.OrdinalIgnoreCase)
                       || NormalizedNickname.Contains(name, StringComparison.OrdinalIgnoreCase))
                   || Username.Contains(name, StringComparison.OrdinalIgnoreCase)
                   || NormalizedUsername.Contains(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}