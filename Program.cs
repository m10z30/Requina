
using Requina.Commands.Helpers;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Number of arguments: " + args.Length);
        var commands = CommandHelper.GetCommands(args);

        for (int i = 0; i < args.Length; i++)
        {
            Console.WriteLine($"Argument {i}: {args[i]}");
        }
    }
}

