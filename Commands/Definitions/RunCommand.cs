using System.Text.Json;
using CommandLine;
using Requina.Common.Constants;
using Requina.Common.Extensions;
using Requina.Core.Endpoints.Helpers;
using Requina.Core.Endpoints.Models;
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
        AppConstants.VariableConstants.BaseDirectory = string.IsNullOrWhiteSpace(options.Directory) ? Directory.GetCurrentDirectory() : options.Directory;

        var activeEnv = EnvHelper.GetActiveEnvironment();
        // await RequestHelper.LogRequestAsync("Test", EndpointMethod.POST, "/api/stuff", async () =>
        // {
        //     await Task.Delay(1000);
        //     var response = new
        //     {
        //         Name = "somename",
        //         Age = 25,
        //     };
        //     return (200, JsonSerializer.Serialize(response));
        // });

        // var envs = EnvHelper.GetEnvironments();
        // Console.WriteLine($"values: {envs.First().Values.Count}");
        // foreach (var env in envs)
        // {
        //     // var values = env.Values;
        //     env.PrintObject();
        // }
        // // var endpoint = EndpointHelper.GetEndpoint("/Users/mohammedzohair/personal/exampleRequinaProject/src/something.ren");
        // endpoint.PrintObject();
        await Task.CompletedTask;
        // ReadEndpoint();
        return 0;
    }
}
