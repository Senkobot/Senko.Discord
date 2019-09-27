using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class GuildMemberUpdateEventArgs
	{
		[DataMember(Name ="guild_id", Order = 1)]
		public ulong GuildId;

		[DataMember(Name ="roles", Order = 2)]
		public ulong[] RoleIds;

		[DataMember(Name ="user", Order = 3)]
		public DiscordUserPacket User;

		[DataMember(Name ="nick", Order = 4)]
		public string Nickname;
	}
}