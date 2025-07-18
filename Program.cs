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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("error: ");
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            return 1;
        }
    }
}

