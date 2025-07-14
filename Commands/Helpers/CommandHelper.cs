using Requina.Commands.Models;

namespace Requina.Commands.Helpers;

public class ArgumentToken
{
    public string Value { get; set; } = string.Empty;
    public int Index { get; set; }
}

public static class CommandHelper
{
    public static readonly CommandDescription[] AvailableCommands = [
        new() {
            Name = "init",
        },
        new() {
            Name = "run"
        }
    ];

    public static List<ArgumentToken> ParseArguments(string[] args)
    {
        var result = new List<ArgumentToken>();
        for (var i = 0; i < args.Length; i++)
        {
            result.Add(new()
            {
                Value = args[i],
                Index = i,
            });
        }
        return result;
    }
    public static Command[] GetCommands(string[] args)
    {
        var tokens = ParseArguments(args);
        return ParseCommands(tokens);
    }

    private static Command[] ParseCommands(List<ArgumentToken> tokens)
    {
        var argumentNames = AvailableCommands.Where(x => !string.IsNullOrEmpty(x.ShortName)).Select(x => x.Name);
        var shortArgmentNames = AvailableCommands.Where(x => !string.IsNullOrEmpty(x.ShortName)).Select(x => x.ShortName);
        var noDashArgumentNames = AvailableCommands.Where(x => !string.IsNullOrEmpty(x.NoDashName)).Select(x => x.NoDashName);
        var argumentNamesTokens = tokens.Where(x => argumentNames.Contains(x.Value)).ToList();
        var shortArgumentTokens = tokens.Where(x => shortArgmentNames.Contains(x.Value)).ToList();
        var noDashTokens = tokens.Where(x => noDashArgumentNames.Contains(x.Value)).ToList();
        // validate short arguments
        var result = new List<Command>();
        foreach (var shortArg in shortArgumentTokens)
        {
            var value = shortArg.Value;
            var lastToken = value.LastOrDefault();
            if (lastToken == '=')
            {
                value = value.Where(x => x != '=').ToString();
                var nextToken = tokens.Where(x => x.Index == shortArg.Index + 1).FirstOrDefault();
                if (nextToken == null)
                {
                    throw new Exception($"{value} need to be assigned a value");
                }
                if (nextToken.Value.Length > 1)
                {
                    var valueToken = tokens.Where(x => x.Index == nextToken.Index + 1).FirstOrDefault();
                    if (valueToken is null)
                    {
                        throw new Exception($"{value} need to be assigned a value");
                    }
                    result.Add(new()
                    {

                    });
                }
            }
        }
        return result.ToArray();
    }
}
