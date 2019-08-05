using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Senko.Discord.Json.Formatters;
using SpanJson;
using SpanJson.Resolvers;

namespace Senko.Discord.Rest
{
    public static class JsonHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySpan<byte> data)
        {
            return JsonSerializer.Generic.Utf8.Deserialize<T, DiscordRestResolver<byte>>(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> SerializeFromPool<T>(T msg)
        {
            return JsonSerializer.Generic.Utf8.SerializeToArrayPool<T, DiscordRestResolver<byte>>(msg);
        }

        public static void ReturnToPool(ArraySegment<byte> data)
        {
            ArrayPool<byte>.Shared.Return(data.Array);
        }
    }

    public sealed class DiscordRestResolver<T> : ResolverBase<T, DiscordRestResolver<T>> where T : struct
    {
        public DiscordRestResolver() : base(new SpanJsonOptions
        {
            EnumOption = EnumOptions.Integer
        })
        {
            RegisterGlobalCustomFormatter<ulong, LongAsStringFormatter>();
            RegisterGlobalCustomFormatter<UserAvatar, UserAvatarFormatter>();
        }
    }
}
