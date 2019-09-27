using System.IO;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    /// <summary>
    /// Data structure to create an emoji.
    /// </summary>
    [DataContract]
    public class EmojiCreationArgs : EmojiModifyArgs
	{
		[DataMember(Name ="image")]
		public Stream Image { get; private set; }

        public EmojiCreationArgs()
        {
        }

        public EmojiCreationArgs(string name, Stream image, params ulong[] roles)
			: base(name, roles)
		{
			Image = image;
		}
	}
}