using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheMule.Shared
{
    public class PrintifyDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        {
            if (reader.TokenType == JsonTokenType.String) {
                string dateTimeString = reader.GetString();
                if (DateTimeOffset.TryParse(dateTimeString, out DateTimeOffset dateTimeOffset)) {
                    return dateTimeOffset;
                }
            }
            throw new JsonException("Unable to parse the date-time string.");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) 
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss zzz"));
        }
    }
}
