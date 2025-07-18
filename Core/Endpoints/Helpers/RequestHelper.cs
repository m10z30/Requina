using System.Diagnostics;
using Requina.Core.Endpoints.Models;

namespace Requina.Core.Endpoints.Helpers;

public static class RequestHelper
{
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
            Console.ResetColor();

            Console.Write($"  Status: {status}  Time: {sw.ElapsedMilliseconds} ms");
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
