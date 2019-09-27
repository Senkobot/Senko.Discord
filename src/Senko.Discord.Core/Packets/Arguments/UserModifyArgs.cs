using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class UserModifyArgs
    {
        [JsonPropertyName("avatar")]
        [DataMember(Name = "avatar")]   
        public UserAvatar Avatar { get; set; }

        [JsonPropertyName("username")]
        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}
