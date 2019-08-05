using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class UserModifyArgs
    {
        [DataMember(Name = "avatar")]   
        public UserAvatar Avatar { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}
