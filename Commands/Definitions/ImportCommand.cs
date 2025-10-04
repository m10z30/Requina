using System.Text.Json;
using CommandLine;
using Requina.Common.Extensions;
using Requina.Core.OpenApi.Models;
using Requina.Helpers.Commands;

namespace Requina.Commands.Definitions;


[Verb("import", false, [], HelpText = "Run the application")]
public class ImportOptions : BaseOptions
{
    [Option('f', "file", Required = true, HelpText = "Specify the path of the openapi json file")]
    public required string FilePath { get; set; }
}

public static class ImportCommand
{
    public static async Task<int> Execute(ImportOptions options)
    {
        if (!File.Exists(options.FilePath))
        {
            throw new Exception("file does not exists");
        }
        var content = await File.ReadAllTextAsync(options.FilePath);
        var _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        var openApiDoc = JsonSerializer.Deserialize<OpenApiDoc>(content, _options);
        if (openApiDoc is null)
        {
            throw new Exception("could not deserialize open api document");
        }
        openApiDoc.PrintObject(maxDepth: 10);
        await Task.CompletedTask;
        return 1;
    }
}
