using CommandLine;
using Requina.Common.Constants;
using Requina.Common.Extensions;
using Requina.Core.Projects.Helpers;
using Requina.Helpers.Commands;

namespace Requina.Commands.Definitions;

// Run command options
[Verb("list", false, ["ls"], HelpText = "Run the application")]
public class ListOptions : BaseOptions
{
    [Option('d', "dir", Required = false, HelpText = "Specify the directory of the requina project")]
    public string? Directory { get; set; }
}

public static class ListCommand
{
    public static async Task<int> Execute(ListOptions options)
    {
        AppConstants.VariableConstants.BaseDirectory = string.IsNullOrWhiteSpace(options.Directory) ? Directory.GetCurrentDirectory() : options.Directory;
        ProjectPrintHelper.PrintProjectStructure(AppConstants.VariableConstants.BaseDirectory);
        await Task.CompletedTask;
        return 0;
    }
}
