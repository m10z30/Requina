using CommandLine;
using Requina.Common.Constants;
using Requina.Core.Endpoints.Helpers;
using Requina.Core.Environments.Helpers;
using Requina.Helpers.Commands;

namespace Requina.Commands.Definitions;

// Run command options
[Verb("run", HelpText = "Run the application")]
public class RunOptions : BaseOptions
{
    [Option('e', "endpoint", Required = false, HelpText = "Specify the endpoint to connect to")]
    public string? Endpoint { get; set; }
    [Option('d', "dir", Required = false, HelpText = "Specify the directory of the requina project")]
    public string? Directory { get; set; }
}

public static class RunCommand
{
    public static async Task<int> Execute(RunOptions options)
    {
        await Task.CompletedTask;
        AppConstants.VariableConstants.BaseDirectory = string.IsNullOrWhiteSpace(options.Directory) ? Directory.GetCurrentDirectory() : options.Directory;
        EnvHelper.GetActiveEnvironment();
        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            return await RunEndpoint(options.Endpoint);
        }
        else
        {
            // TODO: needs to do something here i guess
            throw new Exception("please specify the endpoint to run with -e/--endpoint");
        }
    }

    private static async Task<int> RunEndpoint(string name)
    {
        var endpoint = EndpointHelper.GetEndpointByName(name);
        if (endpoint == null)
        {
            throw new Exception("no endpoint with this name");
        }
        await RequestHelper.Request(endpoint);
        return 0;
    }
}
