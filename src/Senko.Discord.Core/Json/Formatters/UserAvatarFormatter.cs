using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Senko.Discord.Json.Formatters
{
    public sealed class UserAvatarFormatter : JsonConverter<UserAvatar>
    {
        public override UserAvatar Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, UserAvatar value, JsonSerializerOptions options)
        {
            writer.WriteBase64StringValue(value.Stream.GetBuffer());
        }
    }
}
