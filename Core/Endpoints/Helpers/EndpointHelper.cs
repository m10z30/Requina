using Requina.Core.Endpoints.Models;

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
}
