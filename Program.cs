using Requina.Helpers.Commands;

class Program
{
    static async Task<int> Main(string[] args)
    {
        return await CommandInitializer.Execute(args);
    }
}

