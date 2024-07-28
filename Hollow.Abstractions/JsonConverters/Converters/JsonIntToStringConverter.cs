using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hollow.Abstractions.JsonConverters.Converters;

public class JsonIntToStringConverter: JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetUInt64().ToString();
      
        return reader.GetString() ?? "";
    }
   
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}