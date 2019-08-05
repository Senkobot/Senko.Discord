using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class ModifyGuildMemberArgs
	{
		[DataMember(Name ="nick", Order = 1)]
		public string Nickname;

		[DataMember(Name ="roles", Order = 2)]
		public ulong[] RoleIds;

		[DataMember(Name ="mute", Order = 3)]
		public bool? Muted;

		[DataMember(Name ="deaf", Order = 4)]
		public bool? Deafened;

		[DataMember(Name ="channel_id", Order = 5)]
		public ulong? MoveToChannelId;
	}
}