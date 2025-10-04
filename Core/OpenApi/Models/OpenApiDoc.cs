using System.Text.Json.Serialization;

namespace Requina.Core.OpenApi.Models;

public class OpenApiDoc
{
    [JsonPropertyName("x-generator")]
    public string? XGenerator { get; set; }
    [JsonPropertyName("openapi")]
    public required string OpenApi { get; set; }
    [JsonPropertyName("info")]
    public required OpenApiInfo Info { get; set; }
    public OpenApiServers[]? Servers { get; set; }
    public required Dictionary<string, Dictionary<string, OpenApiOperation>> Paths { get; set; }
}

public class OpenApiOperation
{
    public string[] Tags { get; set; } = [];
    public string? OperationId { get; set; }
    public OpenApiParameter[] Parameters { get; set; } = [];
    public OpenApiRequestBody? RequestBody { get; set; }
    public Dictionary<string, OpenApiResponse>? Responses { get; set;  }
}

public class OpenApiResponse
{
    public string? Description { get; set; }
    public Dictionary<string, Dictionary<string, OpenApiScheme>>? Content { get; set; }
}

public class OpenApiRequestBody
{
    [JsonPropertyName("x-name")]
    public string? XName { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, Dictionary<string, OpenApiScheme>>? Content { get; set; }
    public bool Required { get; set; }
    [JsonPropertyName("x-position")]
    public int? XPosition { get; set; }
}

public class OpenApiParameter
{
    public required string Name { get; set; }
    public required string In { get; set; }
    public bool Required { get; set; }
    public OpenApiScheme? Scheme { get; set; }
}

public class OpenApiScheme
{
    public string? Type { get; set; }
    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public class OpenApiInfo
{
    public required string Title { get; set; }
    public string? Version { get; set; }
}


public class OpenApiServers
{
    public string? Url { get; set; }
}
