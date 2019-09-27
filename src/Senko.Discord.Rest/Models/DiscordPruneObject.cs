using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Rest
{
    [DataContract]
    public class DiscordPruneObject
    {
        [JsonPropertyName("pruned")]
        [DataMember(Name = "pruned")]
        public int Pruned { get; set; }
    }
}
