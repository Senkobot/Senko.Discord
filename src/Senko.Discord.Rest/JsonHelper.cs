using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Senko.Discord.Json.Formatters;

namespace Senko.Discord.Rest
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            Converters =
            {
                new LongAsStringFormatter(),
                new UserAvatarFormatter()
            }
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySpan<byte> data)
        {
            return JsonSerializer.Deserialize<T>(data, Options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Serialize<T>(T msg)
        {
            return JsonSerializer.SerializeToUtf8Bytes(msg, Options);
        }
    }
}
