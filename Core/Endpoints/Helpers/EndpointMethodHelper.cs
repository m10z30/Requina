using Requina.Core.Endpoints.Models;

namespace Requina.Core.Endpoints.Helpers;

public static class EndpointMethodHelper
{
    public static bool MethodHasBody(EndpointMethod method)
    {
        return method switch
        {
            EndpointMethod.GET => false,
            EndpointMethod.POST => true,
            EndpointMethod.PUT => true,
            EndpointMethod.PATCH => true,
            EndpointMethod.DELETE => false,
            EndpointMethod.HEAD => false,
            _ => throw new Exception($"endpoint method '{method}' is not handled"),
        };
    }
}
