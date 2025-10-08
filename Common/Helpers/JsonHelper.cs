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

    public static void PrintJsonPretty(string json)
    {
        try
        {
            var parsed = System.Text.Json.JsonDocument.Parse(json);
            var pretty = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(pretty);
            Console.ResetColor();
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(json); // fallback for non-JSON
            Console.ResetColor();
        }
    }

    public static string GetJsonPretty(string json)
    {
        try
        {
            var parsed = System.Text.Json.JsonDocument.Parse(json);
            var pretty = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            return pretty;
        }
        catch
        {
            // fallback for non-JSON
            return json;
        }
    }
}

