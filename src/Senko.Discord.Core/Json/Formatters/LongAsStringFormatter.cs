using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Senko.Discord.Json.Formatters
{
    public sealed class LongAsStringFormatter : JsonConverter<ulong>
    {
        public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (ulong.TryParse(value, out var longValue))
            {
                return longValue;
            }

            throw new InvalidOperationException("Invalid value.");
        }

        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
