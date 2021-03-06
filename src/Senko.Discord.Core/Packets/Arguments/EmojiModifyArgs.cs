﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class EmojiModifyArgs
	{
		[JsonPropertyName("name")]
		[DataMember(Name = "name")]
		public string Name { get; private set; }

		[JsonPropertyName("roles")]
		[DataMember(Name = "roles")]
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