
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class ChannelBulkDeleteArgs
	{
		[DataMember(Name ="messages")]
		public ulong[] Messages { get; set; }

        public ChannelBulkDeleteArgs()
        {
        }

        public ChannelBulkDeleteArgs(ulong[] messages)
		{
			Messages = messages;
		}
	}
}