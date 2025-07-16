using CommandLine;
using Requina.Commands.Definitions;

namespace Requina.Helpers.Commands;

public abstract class BaseOptions
{
}

public static class CommandInitializer
{
    public static async Task<int> Execute(string[] args)
    {
        var parser = new Parser(with =>
        {
            with.HelpWriter = Console.Out;
            with.CaseInsensitiveEnumValues = true;
            with.CaseSensitive = false;
        });

        var result = parser.ParseArguments<InitOptions, RunOptions, ListOptions>(args);

        return await result.MapResult(
            (InitOptions opts) => InitCommand.Execute(opts),
            (RunOptions opts) => RunCommand.Execute(opts),
            (ListOptions opts) => ListCommand.Execute(opts),
            errs => Task.FromResult(1)
        );
    }
}

