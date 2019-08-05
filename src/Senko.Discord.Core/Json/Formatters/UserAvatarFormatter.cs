using System;
using SpanJson;
using SpanJson.Formatters;

namespace Senko.Discord.Json.Formatters
{
    public sealed class UserAvatarFormatter : ICustomJsonFormatter<UserAvatar>
    {
        public static readonly UserAvatarFormatter Default = new UserAvatarFormatter();

        public object Arguments { get; set; }

        public void Serialize(ref JsonWriter<char> writer, UserAvatar value)
        {
            var imageData = Convert.ToBase64String(value.Stream.GetBuffer());
            var data = $"data:image/{value.Type.ToString().ToLower()};base64,{imageData}";

            StringUtf16Formatter.Default.Serialize(ref writer, data);
        }

        public UserAvatar Deserialize(ref JsonReader<char> reader)
        {
            throw new NotSupportedException();
        }

        public void Serialize(ref JsonWriter<byte> writer, UserAvatar value)
        {
            var imageData = Convert.ToBase64String(value.Stream.GetBuffer());
            var data = $"data:image/{value.Type.ToString().ToLower()};base64,{imageData}";

            StringUtf8Formatter.Default.Serialize(ref writer, data);
        }

        public UserAvatar Deserialize(ref JsonReader<byte> reader)
        {
            throw new NotSupportedException();
        }
    }
}
