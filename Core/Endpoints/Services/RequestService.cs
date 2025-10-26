using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Requina.Common.Constants;
using Requina.Common.Helpers;
using Requina.Core.Endpoints.Helpers;
using Requina.Core.Endpoints.Models;
using Requina.Core.Environments.Helpers;
using Requina.Core.Projects.Helpers;

namespace Requina.Core.Endpoints.Services;

public class RequestService
{
    private List<EndpointEvent> events = new();
    private HttpClient client = new HttpClient();

    public async Task Request(Endpoint endpoint)
    {
        events = endpoint.Events;
        client = new HttpClient();
        await LogRequestAsync(endpoint.Name, endpoint.Details.Method, endpoint.Details.Url, async () => await MakeRequest(endpoint));
    }

    public string GetRequestHeader(BodyType bodyType)
    {
        return bodyType switch
        {
            BodyType.Json      => "application/json",
            BodyType.Text      => "text/plain",
            BodyType.FormData  => throw new NotImplementedException(),
            BodyType.XFormData => throw new NotImplementedException(),
            _                  => throw new Exception($"no such bodyType: {bodyType}")
        };
    }

    public async Task<(int status, string responseBody, string? request, string requestHeaders)> MakeRequest(Endpoint endpoint)
    {
        var baseUri = GetBaseUri(endpoint);
        var content = await GetContent(endpoint);
        HttpRequestMessage request = new(GetHttpMethod(endpoint.Details.Method), baseUri)
        {
            Content = content,
        };
        SetHeaders(endpoint, request);
        var activeEnv = EnvHelper.GetActiveEnvironment();
        PreProcessRequest(endpoint, request, activeEnv);
        HttpResponseMessage response = await client.SendAsync(request);
        PostProcessRequest(endpoint, request, activeEnv, response);
        string responseBody = await response.Content.ReadAsStringAsync();
        string? requestContent = null;
        var requestHeaders = request.Headers
            .Select(x => $"{x.Key}: {x.Value.First()}")
            .ToArray();
        var requestHeadersString = string.Join("\n", requestHeaders);
        if (endpoint.Details.Method.HasBody())
        {
            requestContent = await request.Content!.ReadAsStringAsync();
        }
        await endpoint.WriteResponseAsync(response.StatusCode, responseBody);
        return ((int)response.StatusCode, responseBody, requestContent, requestHeadersString);
    }

    private void PreProcessRequest(Endpoint endpoint, HttpRequestMessage request, Environments.Models.Environment environment)
    {
        var context = new ExecutionContext
        {
            Env = environment,
            Request = request
        };
        var eventScriptEngine = new EventScriptEngine(context);
        var beforeEvents = endpoint.Events
            .Where(x => x.Type == EndpointEventType.Before)
            .ToList();
        beforeEvents.AddRange(ProjectHelper.GetGlobalEndpointEvents().Where(x => x.Type == EndpointEventType.Before));
        foreach (var ev in beforeEvents)
        {
            try
            {
                foreach (var line in ev.Lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        eventScriptEngine.Execute(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"error in event syntax in {ev.FilePath}", ex);
            }
        }
    }

    private void PostProcessRequest(Endpoint endpoint, HttpRequestMessage request, Environments.Models.Environment environment, HttpResponseMessage response)
    {
        var context = new ExecutionContext
        {
            Env = environment,
            Request = request,
            Response = response
        };
        var eventScriptEngine = new EventScriptEngine(context);
        var afterEvents = endpoint.Events
            .Where(x => x.Type == EndpointEventType.After)
            .ToList();
        afterEvents.AddRange(ProjectHelper.GetGlobalEndpointEvents().Where(x => x.Type == EndpointEventType.After));
        foreach (var ev in afterEvents)
        {
            try
            {
                foreach (var line in ev.Lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        eventScriptEngine.Execute(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"error in event syntax in {ev.FilePath}", ex);
            }
        }
    }

    private void SetHeaders(Endpoint endpoint, HttpRequestMessage request)
    {
        foreach (var header in endpoint.Details.Headers)
        {
            request.Headers.Add(header.Name, header.Value);
        }
    }

    private async Task<HttpContent?> GetContent(Endpoint endpoint)
    {
        if (endpoint.Details.Method.HasBody())
        {
            return endpoint.Details.Body?.Type switch
            {
                BodyType.Json      => JsonBodyRequestContent(endpoint),
                BodyType.Text      => TextBodyRequestContent(endpoint),
                BodyType.FormData  => await FromDataRequestContentAsync(endpoint),
                BodyType.XFormData => XFormDataRequestContent(endpoint),
                null               => throw new Exception("body type is null"),
                _                  => throw new Exception($"no such request body type: {endpoint.Details.Body.Type}")
            };
        }
        return null;
    }

    private string GetBaseUri(Endpoint endpoint)
    {
        var baseUri = new Uri(endpoint.Details.Url);
        var query = HttpUtility.ParseQueryString(baseUri.Query);
        if (endpoint.Details.Queries != null)
        {
            foreach (var param in endpoint.Details.Queries)
            {
                query[param.Name] = param.Value;
            }
        }
        var uriBuilder = new UriBuilder(baseUri)
        {
            Query = query.ToString()
        };
        return uriBuilder.ToString();
    }

    private HttpMethod GetHttpMethod(EndpointMethod method)
    {
        return method switch
        {
            EndpointMethod.GET    => HttpMethod.Get,
            EndpointMethod.POST   => HttpMethod.Post,
            EndpointMethod.PUT    => HttpMethod.Put,
            EndpointMethod.PATCH  => HttpMethod.Patch,
            EndpointMethod.DELETE => HttpMethod.Delete,
            EndpointMethod.HEAD   => HttpMethod.Head,
            _                     => throw new Exception($"no such method: {method}")
        };
    }

    // private HttpRequestMessage GetRequestAsync(Endpoint endpoint)
    // {
    //     var handler = new HttpClientHandler();
    //     // handler.CookieContainer = new CookieContainer();
    //     // // Add cookies to a specific domain
    //     // var cookie = new System.Net.Cookie("session-id", "abc123", "/", "example.com");
    //     // handler.CookieContainer.Add(cookie);
    //     foreach (var header in endpoint.Details.Headers)
    //     {
    //         client.DefaultRequestHeaders.Add(header.Name, header.Value);
    //     }
    //     var baseUri = new Uri(endpoint.Details.Url);
    //     var query = HttpUtility.ParseQueryString(baseUri.Query);
    //     if (endpoint.Details.Queries != null)
    //     {
    //         foreach (var param in endpoint.Details.Queries)
    //         {
    //             query[param.Name] = param.Value;
    //         }
    //     }
    //     var uriBuilder = new UriBuilder(baseUri)
    //     {
    //         Query = query.ToString()
    //     };
    //     return new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());
    // }

    private StringContent JsonBodyRequestContent(Endpoint endpoint)
    {
        var content = new StringContent(endpoint.Details.Body!.Content!, Encoding.UTF8, GetRequestHeader(endpoint.Details.Body.Type));
        return content;
    }

    private StringContent TextBodyRequestContent(Endpoint endpoint)
    {
        var content = new StringContent(endpoint.Details.Body!.Content!, Encoding.UTF8, GetRequestHeader(endpoint.Details.Body.Type));
        return content;
    }

    private async Task<MultipartFormDataContent> FromDataRequestContentAsync(Endpoint endpoint)
    {
        if (endpoint.Details.Body is null || endpoint.Details.Body.Entries is null)
        {
            throw new Exception($"endpoint '{endpoint.Name}' does not have form data");
        }
        var directoryName = Path.GetDirectoryName(endpoint.FilePath);

        var content = new MultipartFormDataContent();
        foreach (var data in endpoint.Details.Body.Entries)
        {
            var isFile = data.Value.StartsWith('@');
            if (isFile)
            {
                var filePath = data.Value.Where((x, i) => i != 0).ToString();
                if (!FileHelper.IsAbsolutePath(filePath!))
                {
                    filePath = Path.Join(directoryName, filePath);
                }
                if (!File.Exists(filePath))
                {
                    throw new Exception($"file does not exists: {filePath}");
                }
                var fileBytes = await File.ReadAllBytesAsync(filePath!);
                content.Add(new ByteArrayContent(fileBytes), data.Name, Path.GetFileName(filePath!));
            }
            else
            {
                content.Add(new StringContent(data.Name), data.Value);
            }
        }
        return content;
    }

    private FormUrlEncodedContent XFormDataRequestContent(Endpoint endpoint)
    {
        if (endpoint.Details.Body is null || endpoint.Details.Body.Entries is null)
        {
            throw new Exception($"endpoint '{endpoint.Name}' does not have form data");
        }
        var formData = new Dictionary<string, string>();
        foreach (var data in endpoint.Details.Body.Entries)
        {
            formData.Add(data.Name, data.Value);
        }

        var content = new FormUrlEncodedContent(formData);
        return content;
    }

    public static async Task LogRequestAsync(
           string name,
           EndpointMethod method,
           string url,
           Func<Task<(int status, string responseBody, string? request, string requestHeaders)>> requestFunc)
    {
        // Print formatted request line
        Console.Write($"[REQUEST] {name,-25} ");

        Console.ForegroundColor = EndpointMethodHelper.GetMethodColor(method);
        Console.Write($"{method,-8}");
        Console.ResetColor();

        Console.Write($"{url,-40}");

        int cursorLeft = Console.CursorLeft;
        int cursorTop = Console.CursorTop;

        // Spinner setup
        var spinner = new[] { "-", "\\", "|", "/" };
        int spinnerIndex = 0;

        var spinnerTokenSource = new CancellationTokenSource();
        var spinnerTask = Task.Run(async () =>
        {
            while (!spinnerTokenSource.Token.IsCancellationRequested)
            {
                if (!AppConstants.IsDebug)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                }
                Console.Write($"[... {spinner[spinnerIndex++ % spinner.Length]}]");
                await Task.Delay(100);
            }
        });

        var sw = Stopwatch.StartNew();
        var (status, body, request, requestHeaders) = await requestFunc();
        try
        {
            sw.Stop();

            spinnerTokenSource.Cancel();
            await spinnerTask;

            // Move back and print result
            if (!AppConstants.IsDebug)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
            Console.ForegroundColor = status >= 200 && status < 300 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write($"[ {(status >= 200 && status < 300 ? "OK" : "ERR")} ]");
            Console.Write($" {status} {(HttpStatusCode)status}");
            Console.ResetColor();

            Console.Write($" Time: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', 80));
            Console.ResetColor();

            if (!string.IsNullOrEmpty(request))
            {
                Console.WriteLine("Request");
                JsonHelper.PrintJsonPretty(request);
            }
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("Request headers");
            Console.WriteLine();
            Console.WriteLine(requestHeaders);
            Console.WriteLine(new string('-', 80));

            Console.WriteLine("Response");
            JsonHelper.PrintJsonPretty(body);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', 80));
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            spinnerTokenSource.Cancel();
            await spinnerTask;
            sw.Stop();

            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ ERR ]");
            Console.ResetColor();
            Console.Write($"  Time: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }
}
