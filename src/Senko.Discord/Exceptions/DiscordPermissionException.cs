using System;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public class DiscordPermissionException : Exception
	{
        public DiscordPermissionException(GuildPermission permissions)
            : base($"Could not perform actions as permission(s) {permissions} is required.")
        { }

        public DiscordPermissionException(string message)
            : base(message)
        { }
	}
}