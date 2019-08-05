using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordEmbed
	{
		[DataMember(Name ="title", Order = 1)]
		public string Title { get; set; }

        [DataMember(Name = "description", Order = 2)]
		public string Description { get; set; }

		[DataMember(Name ="color", Order = 3)]
		public uint? Color { get; set; } = null;

		[DataMember(Name ="fields", Order = 4)]
		public List<EmbedField> Fields { get; set; }

		[DataMember(Name ="author", Order = 5)]
		public EmbedAuthor Author;

		[DataMember(Name ="footer", Order = 6)]
		public EmbedFooter Footer;

		[DataMember(Name ="thumbnail", Order = 7)]
		public EmbedImage Thumbnail;

		[DataMember(Name ="image", Order = 8)]
		public EmbedImage Image;
	}

    [DataContract]
    public class EmbedAuthor
	{
		[DataMember(Name ="name")]
		public string Name { get; set; }

		[DataMember(Name ="icon_url")]
		public string IconUrl { get; set; }

		[DataMember(Name ="url")]
		public string Url { get; set; }
	}

    [DataContract]
    public class EmbedFooter
	{
		[DataMember(Name ="icon_url")]
		public string IconUrl { get; set; }

		[DataMember(Name ="text")]
		public string Text { get; set; }
	}

    [DataContract]
    public class EmbedImage
	{
		[DataMember(Name ="url")]
		public string Url { get; set; }
	}

    [DataContract]
    public class EmbedField
	{
		[DataMember(Name ="name")]
		public string Title { get; set; }

		[DataMember(Name ="value")]
		public string Content { get; set; }

		[DataMember(Name ="inline")]
		public bool Inline { get; set; } = false;
	}
}