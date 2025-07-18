using Requina.Common.Constants;
using Requina.Core.Endpoints.Models;
using Requina.Core.Environments.Helpers;
using Requina.Core.Projects.Helpers;
using Requina.Core.Projects.Models;
using Requina.Core.Sections.Helpers;
using Requina.Core.Sections.Models;

namespace Requina.Core.Endpoints.Helpers;

public static class EndpointHelper
{

    public static Endpoint? GetEndpointByName(string name)
    {
        var endpoints = GetEndpoints();
        var endpoint = endpoints.Where(x => x.Name == name).FirstOrDefault();
        return endpoint;
    }

    public static List<Endpoint> GetEndpoints()
    {
        var projectStructure = ProjectHelper.GetProjectStructure(AppConstants.VariableConstants.BaseDirectory);
        var endpoints = GetAllEndpointsInDirectory(projectStructure.SourceContent);
        var duplicates = endpoints
            .Select(x => x.Name)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicates.Count != 0)
        {
            var message = $@"Duplicate endpoint names found: {string.Join(", ", duplicates)}
please make sure no two endpoints are the same name, this helps when running a single endpoint
you can specify the name of the endpoint inside the 'info' section with the parameter 'name'";
            throw new Exception(message);
        }
        return endpoints;
    }

    public static List<Endpoint> GetAllEndpointsInDirectory(ProjectDirectory directory)
    {
        var endpoints = new List<Endpoint>();
        foreach (var file in directory.EndpointFiles)
        {
            endpoints.Add(GetEndpoint(file));
        }
        foreach (var dir in directory.Directories)
        {
            endpoints.AddRange(GetAllEndpointsInDirectory(dir));
        }
        return endpoints;
    }

    public static Endpoint GetEndpoint(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new Exception($"file {filePath} does not exists");
        }
        var content = File.ReadAllText(filePath);
        var renderedContent = EnvHelper.RenderContent(content);
        return new()
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            Content = content,
            RenderedContent = renderedContent,
            Sections = SectionHelper.GetSections(renderedContent)
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
        var querySection = GetSection(endpoint, AppConstants.Sections.Query.Name);
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
        var cookiesSection = GetSection(endpoint, AppConstants.Sections.Cookies.Name);
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
        var headerSection = GetSection(endpoint, AppConstants.Sections.Headers.Name);
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
        var bodySection = GetSection(endpoint, AppConstants.Sections.Body.Name);
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
        var urlParameter = infoSection.Parameters.Where(x => x.Name == AppConstants.Sections.Info.Parameters.Url).FirstOrDefault();
        if (urlParameter is null)
        {
            throw new Exception($"in endpoint {endpoint.Name} in info section, url is not defined");
        }
        return urlParameter.Content;
    }

    public static EndpointMethod GetEndpointMethod(Endpoint endpoint)
    {
        var infoSection = GetInfoSection(endpoint);
        var methodParameter = infoSection.Parameters.Where(x => x.Name == AppConstants.Sections.Info.Parameters.Method).FirstOrDefault();
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
        var infoSection = GetSection(endpoint, AppConstants.Sections.Info.Name);
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
        var nameParameter = infoSection.Parameters.Where(x => x.Name == AppConstants.Sections.Info.Parameters.EndpointName).FirstOrDefault();
        if (nameParameter is null)
        {
            return null;
        }
        return nameParameter.Content.Trim(); 
    }
}
