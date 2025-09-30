using System.Text.Json;
using System.Text.RegularExpressions;
using Requina.Common.Extensions;
using Requina.Common.Services;
using Requina.Core.Environments.Extensions;
using Requina.Core.Environments.Helpers;

namespace Requina.Core.Endpoints.Services;

public class ExecutionContext
{
    public required Environments.Models.Environment Env { get; set; }
    public HttpResponseMessage? Response { get; set; }
    public HttpRequestMessage? Request { get; set; }
}

public class EventScriptEngine
{
    private readonly ExecutionContext _ctx;

    public EventScriptEngine(ExecutionContext ctx)
    {
        _ctx = ctx;
    }

    public void Execute(string line)
    {
        // Example: env.name=response.content.name
        var parts = line.Split('=');
        if (parts.Length != 2)
        {
            throw new InvalidOperationException($"Invalid syntax: {line}");
        }

        Logger.LogDebug("preparing to resolve values");

        var target = parts[0];
        var source = parts[1];
        var value = ResolveValue(source);
        AssignValue(target, value);
    }

    private string? ResolveValue(string path)
    {
        var resultValue = "";
        var sources = path.Split(' ');
        for (int i = 0; i < sources.Length; i++)
        {
            var source = sources[i];
            var value = source;
            if (Regex.IsMatch(source, "{*}"))
            {
                value = _ctx.Env.RenderContent(source);
            }
            else if (source.StartsWith("env."))
            {
                var key = source.Substring(4);
                value = _ctx.Env.GetValue(key);
            }
            else if (source.StartsWith("response.content."))
            {
                var key = source.Substring("response.content.".Length);
                var json = _ctx.Response?.Content.ReadAsStringAsync().Result;
                var doc = JsonDocument.Parse(json ?? "{}");
                value = doc.RootElement.GetProperty(key).ToString();
            }
            if (i + 1 == sources.Length)
            {
                resultValue += value;
            }
            else
            {
                resultValue += $"{value} ";
            }
        }
        return resultValue;
    }

    private void AssignValue(string path, string? value)
    {
        if (path.StartsWith("env."))
        {
            var key = path.Substring(4);
            _ctx.Env.UpdateEnvironment(key, (string)value!);
            return;
        }
        if (path.StartsWith("request.headers["))
        {
            var headerName = path.Substring("request.headers[".Length).TrimEnd(']').Trim('"');
            _ctx.Request?.Headers.Add(headerName, value?.ToString() ?? "");
            return;
        }
        throw new NotSupportedException($"Unknown target: {path}");
    }
}

