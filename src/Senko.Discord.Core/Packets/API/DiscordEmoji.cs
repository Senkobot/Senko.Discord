﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordEmoji
	{
		[JsonPropertyName("id")]
        [DataMember(Name = "id", Order = 1)]
		public ulong? Id { get; set; }

		[JsonPropertyName("name")]
        [DataMember(Name = "name", Order = 2)]
		public string Name { get; set; }

		[JsonPropertyName("roles")]
        [DataMember(Name = "roles", Order = 3)]
		public List<ulong> WhitelistedRoles { get; set; }

		[JsonPropertyName("user")]
        [DataMember(Name = "user", Order = 4)]
		public DiscordUserPacket Creator { get; set; }

		[JsonPropertyName("require_colons")]
		[DataMember(Name = "require_colons", Order = 5)]
		public bool? RequireColons { get; set; }

		[JsonPropertyName("managed")]
		[DataMember(Name = "managed", Order = 6)]
		public bool? Managed { get; set; }

		[JsonPropertyName("animated")]
        [DataMember(Name = "animated", Order = 7)]
		public bool? Animated { get; set; }

		public static bool TryParse(string text, out DiscordEmoji emoji)
		{
			emoji = null;
			if (text.Length >= 4 && text[0] == '<' && (text[1] == ':' || (text[1] == 'a' && text[2] == ':')) && text[text.Length - 1] == '>')
			{
				bool animated = text[1] == 'a';
				int startIndex = animated ? 3 : 2;

				int splitIndex = text.IndexOf(':', startIndex);
				if (splitIndex == -1)
				{
					return false;
				}

				if (!ulong.TryParse(text.Substring(splitIndex + 1, text.Length - splitIndex - 2), NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
				{
					return false;
				}

				string name = text.Substring(startIndex, splitIndex - startIndex);

				emoji = new DiscordEmoji
				{
					Name = name,
					Id = id,
					Animated = animated
				};
				return true;
			}
			else if (text.Length > 0 && text.All((t) => char.IsSurrogate(t)))
			{
				emoji = new DiscordEmoji
				{
					Id = null,
					Name = text
				};
				return true;
			}
			return false;
		}

        public override string ToString()
        {
            if(Id.HasValue)
            {
                return $"{Name}:{Id}";
            }
            return Name;
        }
	}
}