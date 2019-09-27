using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class RoleDeleteEventArgs
	{
		[DataMember(Name ="guild_id", Order = 1)]
		public ulong GuildId { get; set; }

		[DataMember(Name ="role_id", Order = 2)]
		public ulong RoleId { get; set; }
	}
}