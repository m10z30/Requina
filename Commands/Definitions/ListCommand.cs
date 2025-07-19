using CommandLine;
using Requina.Common.Constants;
using Requina.Core.Endpoints.Helpers;
using Requina.Core.Environments.Helpers;
using Requina.Core.Projects.Helpers;
using Requina.Helpers.Commands;

namespace Requina.Commands.Definitions;

// Run command options
[Verb("list", false, ["ls"], HelpText = "Run the application")]
public class ListOptions : BaseOptions
{
    [Option('d', "dir", Required = false, HelpText = "Specify the directory of the requina project")]
    public string? Directory { get; set; }
    [Option('t', "tree", Required = false, HelpText = "Print the project as tree")]
    public bool? Tree { get; set; }
}

public static class ListCommand
{
    public static async Task<int> Execute(ListOptions options)
    {
        AppConstants.VariableConstants.BaseDirectory = string.IsNullOrWhiteSpace(options.Directory) ? Directory.GetCurrentDirectory() : options.Directory;
        await EnvHelper.UpdateActiveEnvironmentAsync("something", "something");
        return 0;
        // if (options.Tree == true)
        // {
        //     ProjectPrintHelper.PrintProjectStructure(AppConstants.VariableConstants.BaseDirectory);
        //     return 0;
        // }
        // var endpoints = EndpointHelper.GetEndpoints();
        // Console.WriteLine($"{"Name",-30} {"Method",-10} {"URL"}");
        // Console.WriteLine(new string('-', 60));

        // foreach (var endpoint in endpoints)
        // {
        //     Console.Write($"{endpoint.Name,-30} ");

        //     Console.ForegroundColor = EndpointMethodHelper.GetMethodColor(endpoint.Details.Method);
        //     Console.Write($"{endpoint.Details.Method,-10}");
        //     Console.ResetColor();

        //     Console.WriteLine($" {endpoint.Details.Url}");
        // }
        // await Task.CompletedTask;
        // return 0;
    }
}
