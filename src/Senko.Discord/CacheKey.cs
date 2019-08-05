using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Senko.Discord
{
	internal static class CacheKey
    {
        private const string ChannelPrefix = "discord:channel";

        private const string UserPrefix = "discord:user";

        private const string GuildPrefix = "discord:guild";

        private const string GuildMemberPrefix = "discord:guild_member";

        private const string GuildRolePrefix = "discord:guild_role";

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Guild(ulong id)
        {
            return $"{GuildPrefix}:{id}";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Channel(ulong? channelId = null)
        {
            return channelId.HasValue ? Channel(channelId.Value) : $"{ChannelPrefix}:dm";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ChannelIdList(ulong guildId)
        {
            return $"{ChannelPrefix}:_ids";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Channel(ulong channelId)
        {
            return $"{ChannelPrefix}:{channelId}";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GuildMemberIdList(ulong guildId)
        {
            return $"{GuildMemberPrefix}:{guildId}:_ids";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GuildMember(ulong guildId, ulong userId)
        {
            return $"{GuildMemberPrefix}:{guildId}:{userId}";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GuildRoleIdList(ulong guildId)
        {
            return $"{GuildRolePrefix}:{guildId}:_ids";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GuildRole(ulong guildId, ulong roleId)
        {
            return $"{GuildRolePrefix}:{guildId}:{roleId}";
        }

        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string User(ulong id)
        {
            return $"{UserPrefix}:{id}";
        }
    }
}