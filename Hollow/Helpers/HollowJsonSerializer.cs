using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hollow.Helpers;

public class HollowJsonSerializer
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}