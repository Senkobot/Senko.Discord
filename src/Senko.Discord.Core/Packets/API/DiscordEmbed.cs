using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordEmbed
	{
		[JsonPropertyName("title")]
		[DataMember(Name = "title", Order = 1)]
		public string Title { get; set; }

        [JsonPropertyName("description")]
        [DataMember(Name = "description", Order = 2)]
		public string Description { get; set; }

		[DataMember(Name = "color", Order = 3)]
		public uint? Color { get; set; }

		[JsonPropertyName("fields")]
		[DataMember(Name = "fields", Order = 4)]
		public List<EmbedField> Fields { get; set; }

		[JsonPropertyName("author")]
		[DataMember(Name = "author", Order = 5)]
		public EmbedAuthor Author { get; set; }

        [JsonPropertyName("footer")]
		[DataMember(Name = "footer", Order = 6)]
		public EmbedFooter Footer { get; set; }

        [JsonPropertyName("thumbnail")]
		[DataMember(Name = "thumbnail", Order = 7)]
		public EmbedImage Thumbnail { get; set; }

        [JsonPropertyName("image")]
		[DataMember(Name = "image", Order = 8)]
		public EmbedImage Image { get; set; }
    }

    [DataContract]
    public class EmbedAuthor
	{
		[JsonPropertyName("name")]
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[JsonPropertyName("icon_url")]
		[DataMember(Name = "icon_url")]
		public string IconUrl { get; set; }

		[JsonPropertyName("url")]
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

    [DataContract]
    public class EmbedFooter
	{
		[JsonPropertyName("icon_url")]
		[DataMember(Name = "icon_url")]
		public string IconUrl { get; set; }

		[JsonPropertyName("text")]
		[DataMember(Name = "text")]
		public string Text { get; set; }
	}

    [DataContract]
    public class EmbedImage
	{
		[JsonPropertyName("url")]
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

    [DataContract]
    public class EmbedField
	{
		[JsonPropertyName("name")]
		[DataMember(Name = "name")]
		public string Title { get; set; }

		[JsonPropertyName("value")]
		[DataMember(Name = "value")]
		public string Content { get; set; }

		[JsonPropertyName("inline")]
		[DataMember(Name = "inline")]
		public bool Inline { get; set; }
	}
}