using CommandLine;
using Requina.Common.Constants;
using Requina.Core.Endpoints.Helpers;
using Requina.Core.Endpoints.Services;
using Requina.Core.Environments.Helpers;
using Requina.Helpers.Commands;

namespace Requina.Commands.Definitions;

// Run command options
[Verb("run", HelpText = "Run the application")]
public class RunOptions : BaseOptions
{
    [Value(0, MetaName = "endpoint", HelpText = "Specify the endpoint to run", Required = true)]
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
        EnvHelper.GetActiveEnvironment(true);
        Console.WriteLine(options.Directory);
        Console.WriteLine(options.Endpoint);
        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            return await RunEndpoint(options.Endpoint);
        }
        else
        {
            // TODO: needs to do something here i guess
            throw new Exception("please specify the endpoint to run");
        }
    }

    private static async Task<int> RunEndpoint(string name)
    {
        var endpoint = EndpointHelper.GetEndpointByName(name);
        if (endpoint == null)
        {
            throw new Exception("no endpoint with this name");
        }
        var requestService = new RequestService();
        await requestService.Request(endpoint);
        return 0;
    }
}
