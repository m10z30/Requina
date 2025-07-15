using Requina.Common.Constants;
using Requina.Core.Endpoints.Models;
using Requina.Core.Sections.Models;

namespace Requina.Core.Endpoints.Helpers;

public static class EndpointHelper
{
    public static Endpoint GetEndpoint(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new Exception($"file {filePath} does not exists");
        }
        return new()
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            Content = File.ReadAllText(filePath),
        };
    }

    public static EndpointDetails GetDetails(Endpoint endpoint)
    {
        var method = GetEndpointMethod(endpoint);
        if (method.HasBody())
        {
            var headers = GetEndpointHeaders(endpoint);
            var body = GetEndpointBody(endpoint);
            var cookies = GetEndpointCookies(endpoint);
            var queries = GetEndpointQueries(endpoint);
            return new()
            {
                Url = GetEndpointUrl(endpoint),
                Method = method,
                Headers = headers,
                Body = body,
                Cookies = cookies,
                Queries = queries,
            };
        }
        else
        {
            var headers = GetEndpointHeaders(endpoint);
            var cookies = GetEndpointCookies(endpoint);
            var queries = GetEndpointQueries(endpoint);
            return new()
            {
                Url = GetEndpointUrl(endpoint),
                Method = method,
                Headers = headers,
                Cookies = cookies,
                Queries = queries,
            };
        }
    }

    public static List<Query>? GetEndpointQueries(Endpoint endpoint)
    {
        var result = new List<Query>();
        var querySection = GetSection(endpoint, EndpointTokens.Sections.Query.Name);
        if (querySection is null)
        {
            return null;
        }
        foreach (var parameter in querySection.Parameters)
        {
            result.Add(new()
            {
                Name = parameter.Name,
                Value = parameter.Content
            });
        }
        return result;
    }

    public static List<Cookie>? GetEndpointCookies(Endpoint endpoint)
    {
        var result = new List<Cookie>();
        var cookiesSection = GetSection(endpoint, EndpointTokens.Sections.Cookies.Name);
        if (cookiesSection is null)
        {
            return null;
        }
        foreach (var parameter in cookiesSection.Parameters)
        {
            result.Add(new()
            {
                Name = parameter.Name,
                Value = parameter.Content
            });
        }
        return result;
    }

    public static List<Header> GetEndpointHeaders(Endpoint endpoint)
    {
        var result = new List<Header>();
        var headerSection = GetSection(endpoint, EndpointTokens.Sections.Headers.Name);
        if (headerSection is null)
        {
            throw new Exception($"endpoint '{endpoint.Name}' does not have headers");
        }
        foreach (var parameter in headerSection.Parameters)
        {
            result.Add(new()
            {
                Name = parameter.Name,
                Value = parameter.Content
            });
        }
        return result;
    }

    public static List<BodyEntry> GetEndpointBody(Endpoint endpoint)
    {
        var result = new List<BodyEntry>();
        var bodySection = GetSection(endpoint, EndpointTokens.Sections.Body.Name);
        if (bodySection is null)
        {
            throw new Exception($"endpoint '{endpoint.Name}' does not have a body");
        }
        foreach (var parameter in bodySection.Parameters)
        {
            result.Add(new()
            {
                Name = parameter.Name,
                Value = parameter.Content
            });
        }
        return result;
    }

    public static string GetEndpointUrl(Endpoint endpoint)
    {
        var infoSection = GetInfoSection(endpoint);
        var urlParameter = infoSection.Parameters.Where(x => x.Name == EndpointTokens.Sections.Info.Parameters.Url).FirstOrDefault();
        if (urlParameter is null)
        {
            throw new Exception($"in endpoint {endpoint.Name} in info section, url is not defined");
        }
        return urlParameter.Content;
    }

    public static EndpointMethod GetEndpointMethod(Endpoint endpoint)
    {
        var infoSection = GetInfoSection(endpoint);
        var methodParameter = infoSection.Parameters.Where(x => x.Name == EndpointTokens.Sections.Info.Parameters.Method).FirstOrDefault();
        if (methodParameter is null)
        {
            throw new Exception($"in endpoint {endpoint.Name} in info section, method is not defined!");
        }
        if (!Enum.TryParse<EndpointMethod>(methodParameter.Content.ToUpper(), out var method))
        {
            throw new Exception($"in endpoint {endpoint.Name} in info section, method '{methodParameter.Content}' is not a valid http method");
        }
        return method;
    }
    
    public static Section GetInfoSection(Endpoint endpoint)
    {
        var infoSection = GetSection(endpoint, EndpointTokens.Sections.Info.Name);
        if (infoSection is null)
        {
            throw new Exception($"in endpoint {endpoint.Name} info section does not exists");
        }
        return infoSection;
    }
    
    public static Section? GetSection(Endpoint endpoint, string name)
    {
        var section = endpoint.Sections
            .Where(x => x.Name == name)
            .FirstOrDefault();
        return section;
    }

    public static string? GetInfoName(Endpoint endpoint)
    {
        var infoSection = GetInfoSection(endpoint);
        var nameParameter = infoSection.Parameters.Where(x => x.Name == EndpointTokens.Sections.Info.Parameters.EndpointName).FirstOrDefault();
        if (nameParameter is null)
        {
            return null;
        }
        return nameParameter.Content; 
    }
}
