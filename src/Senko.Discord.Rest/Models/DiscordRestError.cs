using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Rest
{
    [DataContract]
    public class DiscordRestError
    {
        [JsonPropertyName("code")]
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        [DataMember(Name = "message")]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Code}: {Message}\n";
        }
    }
}
