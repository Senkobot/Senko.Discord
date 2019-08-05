using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class EditMessageArgs
    {
        public EditMessageArgs()
        {
        }

        public EditMessageArgs(string content = null, DiscordEmbed embed = null)
        {
            Content = content;
            Embed = embed;
        }

        [DataMember(Name = "content")]
        public string Content;

        [DataMember(Name = "embed")]
        public DiscordEmbed Embed;
    }

    [DataContract]
    public class MessageArgs : EditMessageArgs
	{
        public MessageArgs()
        {
        }

        public MessageArgs(string content = null, DiscordEmbed embed = null, bool tts = false) 
            : base(content, embed)
        {
            TextToSpeech = tts;
        }

        [DataMember(Name = "tts")]
        public bool TextToSpeech;
	}
}