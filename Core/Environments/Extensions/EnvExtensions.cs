namespace Requina.Core.Environments.Extensions;

public static class EnvExtensions
{
    public static string RenderContent(this Models.Environment env, string content)
    {
        foreach (var value in env.Values)
        {
            content = content.Replace('{' + value.Name + '}', value.Value);
        }
        return content;
    }
}
