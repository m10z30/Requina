using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using Requina.Common.Helpers;
using Requina.Core.Endpoints.Models;

namespace Requina.Core.Endpoints.Helpers;

public static class RequestHelper
{

    public static async Task Request(Endpoint endpoint)
    {
        await LogRequestAsync(endpoint.Name, endpoint.Details.Method, endpoint.Details.Url, async () => await MakeRequest(endpoint));
    }

    public static string GetRequestHeader(BodyType bodyType)
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

    public static async Task<(int status, string responseBody)> MakeRequest(Endpoint endpoint)
    {
        HttpResponseMessage response = endpoint.Details.Method switch
        {
            EndpointMethod.GET    => await GetRequestAsync(endpoint),
            EndpointMethod.POST   => await PostRequestAsync(endpoint),
            EndpointMethod.PUT    => await PutRequestAsync(endpoint),
            EndpointMethod.PATCH  => await PatchRequestAsync(endpoint),
            EndpointMethod.DELETE => await DeletRequestAsync(endpoint),
            EndpointMethod.HEAD   => await HeadRequestAsync(endpoint),
            _                     => throw new Exception($"no such method: {endpoint.Details.Method}")
        };
        string responseBody = await response.Content.ReadAsStringAsync();
        return ((int)response.StatusCode, responseBody);
    }

    private static async Task<HttpResponseMessage> GetRequestAsync(Endpoint endpoint)
    {
        var handler = new HttpClientHandler();
        // handler.CookieContainer = new CookieContainer();
        // // Add cookies to a specific domain
        // var cookie = new System.Net.Cookie("session-id", "abc123", "/", "example.com");
        // handler.CookieContainer.Add(cookie);
        using HttpClient client = new(handler);
        foreach (var header in endpoint.Details.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
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
        return await client.GetAsync(uriBuilder.ToString());
    }

    private static async Task<HttpResponseMessage> PostRequestAsync(Endpoint endpoint)
    {
        return endpoint.Details.Body?.Type switch
        {
            BodyType.Json => await JsonBodyRequestAsync(endpoint),
            BodyType.Text => await TextBodyRequestAsync(endpoint),
            BodyType.FormData => await FormDataRequestAsync(endpoint),
            BodyType.XFormData => await XFormDataRequestAsync(endpoint),
            _ => throw new Exception($"no body type specified for '{endpoint.Name}' endpoint"),
        };
    }

    private static async Task<HttpResponseMessage> JsonBodyRequestAsync(Endpoint endpoint)
    {
        using var client = new HttpClient();
        foreach (var header in endpoint.Details.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
        var content = new StringContent(endpoint.Details.Body!.Content!, Encoding.UTF8, GetRequestHeader(endpoint.Details.Body.Type));
        return endpoint.Details.Method switch
        {
            EndpointMethod.POST  => await client.PostAsync(endpoint.Details.Url, content),
            EndpointMethod.PUT   => await client.PutAsync(endpoint.Details.Url, content),
            EndpointMethod.PATCH => await client.PatchAsync(endpoint.Details.Url, content),
            _                    => throw new Exception($"this type of endpoint does not have body: [{endpoint.Details.Method}] '{endpoint.Name}'"),
        };
    }

    private static async Task<HttpResponseMessage> TextBodyRequestAsync(Endpoint endpoint)
    {
        using var client = new HttpClient();
        foreach (var header in endpoint.Details.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
        var content = new StringContent(endpoint.Details.Body!.Content!, Encoding.UTF8, GetRequestHeader(endpoint.Details.Body.Type));
        return endpoint.Details.Method switch
        {
            EndpointMethod.POST  => await client.PostAsync(endpoint.Details.Url, content),
            EndpointMethod.PUT   => await client.PutAsync(endpoint.Details.Url, content),
            EndpointMethod.PATCH => await client.PatchAsync(endpoint.Details.Url, content),
            _                    => throw new Exception($"this type of endpoint does not have body: [{endpoint.Details.Method}] '{endpoint.Name}'"),
        };
    }

    private static async Task<HttpResponseMessage> FormDataRequestAsync(Endpoint endpoint)
    {
        if (endpoint.Details.Body is null || endpoint.Details.Body.Entries is null)
        {
            throw new Exception($"endpoint '{endpoint.Name}' does not have form data");
        }
        var directoryName = Path.GetDirectoryName(endpoint.FilePath);
        using var client = new HttpClient();

        foreach (var header in endpoint.Details.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
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
        return endpoint.Details.Method switch
        {
            EndpointMethod.POST  => await client.PostAsync(endpoint.Details.Url, content),
            EndpointMethod.PUT   => await client.PutAsync(endpoint.Details.Url, content),
            EndpointMethod.PATCH => await client.PatchAsync(endpoint.Details.Url, content),
            _                    => throw new Exception($"this type of endpoint does not have body: [{endpoint.Details.Method}] '{endpoint.Name}'"),
        };
    }


    private static async Task<HttpResponseMessage> XFormDataRequestAsync(Endpoint endpoint)
    {
        using var client = new HttpClient();

        foreach (var header in endpoint.Details.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
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

        return endpoint.Details.Method switch
        {
            EndpointMethod.POST  => await client.PostAsync(endpoint.Details.Url, content),
            EndpointMethod.PUT   => await client.PutAsync(endpoint.Details.Url, content),
            EndpointMethod.PATCH => await client.PatchAsync(endpoint.Details.Url, content),
            _                    => throw new Exception($"this type of endpoint does not have body: [{endpoint.Details.Method}] '{endpoint.Name}'"),
        };
    }


    private static async Task<HttpResponseMessage> PutRequestAsync(Endpoint endpoint)
    {
        return endpoint.Details.Body?.Type switch
        {
            BodyType.Json => await JsonBodyRequestAsync(endpoint),
            BodyType.Text => await TextBodyRequestAsync(endpoint),
            BodyType.FormData => await FormDataRequestAsync(endpoint),
            BodyType.XFormData => await XFormDataRequestAsync(endpoint),
            _ => throw new Exception($"no body type specified for '{endpoint.Name}' endpoint"),
        };
    }

    private static async Task<HttpResponseMessage> PatchRequestAsync(Endpoint endpoint)
    {
        return endpoint.Details.Body?.Type switch
        {
            BodyType.Json => await JsonBodyRequestAsync(endpoint),
            BodyType.Text => await TextBodyRequestAsync(endpoint),
            BodyType.FormData => await FormDataRequestAsync(endpoint),
            BodyType.XFormData => await XFormDataRequestAsync(endpoint),
            _ => throw new Exception($"no body type specified for '{endpoint.Name}' endpoint"),
        };
    }

    private static async Task<HttpResponseMessage> DeletRequestAsync(Endpoint endpoint)
    {
        var handler = new HttpClientHandler();
        // handler.CookieContainer = new CookieContainer();
        // // Add cookies to a specific domain
        // var cookie = new System.Net.Cookie("session-id", "abc123", "/", "example.com");
        // handler.CookieContainer.Add(cookie);
        using HttpClient client = new(handler);
        foreach (var header in endpoint.Details.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
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
        return await client.DeleteAsync(uriBuilder.ToString());
    }

    private static async Task<HttpResponseMessage> HeadRequestAsync(Endpoint endpoint)
    {
        var handler = new HttpClientHandler();
        // handler.CookieContainer = new CookieContainer();
        // // Add cookies to a specific domain
        // var cookie = new System.Net.Cookie("session-id", "abc123", "/", "example.com");
        // handler.CookieContainer.Add(cookie);
        using HttpClient client = new(handler);
        foreach (var header in endpoint.Details.Headers)
        {
            client.DefaultRequestHeaders.Add(header.Name, header.Value);
        }
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
        var request = new HttpRequestMessage(HttpMethod.Head, uriBuilder.ToString());
        return await client.SendAsync(request);
    }

    public static async Task LogRequestAsync(
           string name,
           EndpointMethod method,
           string url,
           Func<Task<(int status, string responseBody)>> requestFunc)
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
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.Write($"[... {spinner[spinnerIndex++ % spinner.Length]}]");
                await Task.Delay(100);
            }
        });

        var sw = Stopwatch.StartNew();

        try
        {
            var (status, body) = await requestFunc();
            sw.Stop();

            spinnerTokenSource.Cancel();
            await spinnerTask;

            // Move back and print result
            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.ForegroundColor = status >= 200 && status < 300 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write($"[ {(status >= 200 && status < 300 ? "OK" : "ERR")} ]");
            Console.Write($" {status} {(HttpStatusCode)status}");
            Console.ResetColor();

            Console.Write($" Time: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', 80));
            Console.ResetColor();

            PrintJsonPretty(body);

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

    private static void PrintJsonPretty(string json)
    {
        try
        {
            var parsed = System.Text.Json.JsonDocument.Parse(json);
            var pretty = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(pretty);
            Console.ResetColor();
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(json); // fallback for non-JSON
            Console.ResetColor();
        }
    }
}
