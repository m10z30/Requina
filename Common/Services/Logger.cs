using Requina.Common.Constants;

namespace Requina.Common.Services;

public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
}

public static class Logger
{
    public static ConsoleColor GetColor(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => ConsoleColor.DarkMagenta,
            LogLevel.Info  => ConsoleColor.Green,
            LogLevel.Warn  => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _              => throw new Exception($"log level is implemented: {nameof(logLevel)}"),
        };
    }

    public static void LogDebug(string message)
    {
        if (AppConstants.IsDebug)
        {
            Console.ForegroundColor = GetColor(LogLevel.Debug); 
            Console.Write("debug: ");
            Console.ResetColor();
            Console.WriteLine(message);
        }
    }

    public static void LogInfo(string message)
    {
        Console.ForegroundColor = GetColor(LogLevel.Info); 
        Console.Write("info: ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    public static void LogWarn(string message)
    {
        Console.ForegroundColor = GetColor(LogLevel.Warn); 
        Console.Write("warn: ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    public static void LogError(string message)
    {
        Console.ForegroundColor = GetColor(LogLevel.Error); 
        Console.Write("\nerror: ");
        Console.ResetColor();
        Console.WriteLine(message);
    }
}
