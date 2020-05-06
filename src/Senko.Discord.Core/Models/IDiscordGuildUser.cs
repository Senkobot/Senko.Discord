using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public interface IDiscordGuildUser : IDiscordUser
	{
		string Nickname { get; }

		IReadOnlyCollection<ulong> RoleIds { get; }

		ulong GuildId { get; }

		DateTimeOffset JoinedAt { get; }

        /// <summary>
        /// This user nitro boosting current 
        /// </summary>
        DateTimeOffset? PremiumSince { get; }

		ValueTask AddRoleAsync(IDiscordRole role);

		ValueTask<IDiscordGuild> GetGuildAsync();

		ValueTask<int> GetHierarchyAsync();

		ValueTask<bool> HasPermissionsAsync(GuildPermission permissions);

		ValueTask KickAsync(string reason = "");

		ValueTask RemoveRoleAsync(IDiscordRole role);
	}
}