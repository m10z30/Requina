using Requina.Core.Environments.Helpers;

namespace Requina.Core.Environments.Models;

public class Environment
{
    public required string FilePath { get; set; }
    public required string FileName { get; set; }
    public required string Name { get; set; }
    public required List<EnvValue> Values { get; set; }
}

public class EnvValue
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}