using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordRole : ISnowflake
	{
        /// <summary>
        /// The escaped name of the role.
        /// </summary>
		string Name { get; }
        /// <summary>
        /// The raw name of the role. In the that the role contains '@everyone' and '@here' it'll get pinged.
        /// </summary>
        string RawName { get; }
        /// <summary>
        /// The color attached to the role.
        /// </summary>
		Color Color { get; }
        /// <summary>
        /// The position of the role compared to other roles.
        /// </summary>
		int Position { get; }
        /// <summary>
        /// Permissions attached to the role.
        /// </summary>
		GuildPermission Permissions { get; }
        /// <summary>
        /// Is this role managed by an external service?
        /// </summary>
        bool IsManaged { get; }
        /// <summary>
        /// Is this role hoisted up in the user list?
        /// </summary>
        bool IsHoisted { get; }
        /// <summary>
        /// Can this role be mentioned in a <see cref="IDiscordTextChannel"/>?
        /// </summary>
        bool IsMentionable { get; }
	}
}