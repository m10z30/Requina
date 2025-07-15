using Requina.Core.Endpoints.Helpers;
using Requina.Core.Sections.Helpers;
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
    public required string FileName { get; set; }
    public string Name => Path.GetFileNameWithoutExtension(FilePath);
    public string? InfoName => EndpointHelper.GetInfoName(this);
    public required string Content { get; set; }
    public List<Section> Sections => SectionHelper.GetSections(Content);
    public EndpointDetails Details => EndpointHelper.GetDetails(this);
}

public class EndpointDetails
{
    public string Url { get; set; } = string.Empty;
    public EndpointMethod Method { get; set; }
    public List<Header> Headers { get; set; } = new();
    public List<BodyEntry>? Body { get; set; }
    public List<Query>? Queries { get; set; }
    public List<Cookie>? Cookies { get; set; }
}

public class Header
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
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