﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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

        [JsonPropertyName("content")]
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [JsonPropertyName("embed")]
        [DataMember(Name = "embed")]
        public DiscordEmbed Embed { get; set; }
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

        [JsonPropertyName("tts")]
        [DataMember(Name = "tts")]
        public bool TextToSpeech { get; set; }
    }
}