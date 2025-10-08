using Requina.Common.Helpers;
using Requina.Common.Services;
using Requina.Core.Endpoints.Helpers;
using Requina.Core.Sections.Models;

namespace Requina.Core.Endpoints.Models;

public enum EndpointMethod
{
    GET,
    POST,
    PUT,
    PATCH,
    DELETE,
    HEAD,
}


public class Endpoint
{
    public required string FilePath { get; set; }
    public string Directory => Path.GetDirectoryName(FilePath) ?? throw new Exception($"something went wrong when getting directory name of {FilePath}");
    public required string FileName { get; set; }
    public string Name => InfoName ?? Path.GetFileName(FilePath);
    public string? InfoName => EndpointHelper.GetInfoName(this);
    public required string Content { get; set; }
    public required string RenderedContent { get; set; }
    public required List<Section> Sections { get; set; }
    public EndpointDetails Details => EndpointHelper.GetDetails(this);
    public List<EndpointEvent> Events => EndpointHelper.GetEvents(this);

    public async Task WriteResponseAsync(System.Net.HttpStatusCode statusCode, string responseBody)
    {
        var sectionName = $"Response - {(int)statusCode} {statusCode}";
        var prettyJson = JsonHelper.GetJsonPretty(responseBody);
        Logger.LogDebug($"secionName: {sectionName}");
        var existingSection = Sections.Where(x => x.Name == sectionName).FirstOrDefault();
        if (existingSection != null)
        {
            existingSection.Content = prettyJson;
        }
        else
        {
            Sections.Add(new Section
            {
                Name = sectionName,
                Content = prettyJson,
            });
        }
        await EndpointHelper.UpdateSectionsAsync(this);
    }
}

public enum EndpointEventType
{
    Before,
    After
}

public class EndpointEvent
{
    public required string FilePath { get; set; }
    public EndpointEventType Type { get; set; }
    public List<string> Lines { get; set; } = [];
}

public class EndpointDetails
{
    public string Url { get; set; } = string.Empty;
    public EndpointMethod Method { get; set; }
    public List<Header> Headers { get; set; } = new();
    public Body? Body { get; set; }
    public List<Query>? Queries { get; set; }
    public List<Cookie>? Cookies { get; set; }
}

public class Header
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public enum BodyType
{
    Json,
    Text,
    FormData,
    XFormData
}

public class Body
{
    public BodyType Type { get; set; }
    public string? Content { get; set; }
    public List<BodyEntry>? Entries { get; set; }
}

public class BodyEntry
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class Query
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class Cookie 
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}