using Requina.Common.Extensions;
using Requina.Helpers.Commands;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            return await CommandInitializer.Execute(args);
        }
        catch (Exception ex)
        {
            PrintError(ex);
            return 1;
        }
    }

    static void PrintError(Exception ex, int depth = -1)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        if (depth > -1)
        {
            Console.Write(string.Concat(Enumerable.Repeat("    ", depth)) + "└──>");
            Console.WriteLine(ex.Message);
        }
        else
        {
            Console.WriteLine(ex.Message);
        }
        if (ex.InnerException is not null)
        {
            PrintError(ex.InnerException, depth + 1);
        }
        Console.ResetColor();
    }
}

