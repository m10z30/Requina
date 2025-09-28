using System.Text.Json;

namespace Requina.Common.Helpers;


public static class JsonHelper
{
    public static JsonElement? GetValue(string json, string path)
    {
        using var doc = JsonDocument.Parse(json);
        var arrayPath = path.Split(".").ToArray();
        var value = GetValue(doc.RootElement, arrayPath);
        return value;
    }
    public static JsonElement? GetValue(JsonElement element, string[] path)
    {
        JsonElement current = element;
        foreach (var prop in path)
        {
            if (current.ValueKind == JsonValueKind.Object &&
                current.TryGetProperty(prop, out JsonElement next))
            {
                current = next;
            }
            else
            {
                return null; // property not found
            }
        }
        return current;
    }
}

