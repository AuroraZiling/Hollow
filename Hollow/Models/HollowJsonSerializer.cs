using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hollow.Models;

public class HollowJsonSerializer
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}