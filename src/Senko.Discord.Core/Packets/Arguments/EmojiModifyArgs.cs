using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class EmojiModifyArgs
	{
		[DataMember(Name ="name")]
		public string Name { get; private set; }

		[DataMember(Name ="roles")]
		public ulong[] Roles { get; private set; }

        public EmojiModifyArgs()
        {
        }

        public EmojiModifyArgs(string name, params ulong[] roles)
		{
			Name = name;
			Roles = roles;
		}
	}
}