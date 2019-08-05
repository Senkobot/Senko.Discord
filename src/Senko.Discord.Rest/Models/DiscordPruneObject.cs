using System.Runtime.Serialization;

namespace Senko.Discord.Rest
{
    [DataContract]
    public class DiscordPruneObject
    {
        [DataMember(Name = "pruned")]
        public int Pruned { get; set; }
    }
}
