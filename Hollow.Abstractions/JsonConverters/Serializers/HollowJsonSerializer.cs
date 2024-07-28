using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hollow.Abstractions.JsonConverters.Serializers;

public class HollowJsonSerializer
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}