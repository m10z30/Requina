using System.Text.Json;
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
        // Example: set env.name response.content.name
        var parts = line.Split(' ', 3);
        if (parts.Length != 3 || parts[0] != "set")
        {
            throw new InvalidOperationException($"Invalid syntax: {line}");
        }

        var target = parts[1];
        var source = parts[2];
        var value = ResolveValue(source);
        AssignValue(target, value);
    }

    private object? ResolveValue(string path)
    {
        if (path.StartsWith("env."))
        {
            var key = path.Substring(4);
            return _ctx.Env.GetValue(key);
        }
        if (path.StartsWith("response.content."))
        {
            var key = path.Substring("response.content.".Length);
            var json = _ctx.Response?.Content.ReadAsStringAsync().Result;
            var doc = JsonDocument.Parse(json ?? "{}");
            return doc.RootElement.GetProperty(key).ToString();
        }
        else
        {
            return path;
        }
        throw new NotSupportedException($"Unknown source: {path}");
    }

    private void AssignValue(string path, object? value)
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

