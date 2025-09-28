namespace Requina.Core.Environments.Models;

public class Environment
{
    public required string FilePath { get; set; }
    public required string FileName { get; set; }
    public required string Name { get; set; }
    public required bool IsActive { get; set; }
    public required List<EnvValue> Values { get; set; }
    public string? GetValue(string name)
    {
        var value = Values.Where(x => x.Name == name).FirstOrDefault();
        return value?.Value ?? null;
    }
}

public class EnvValue
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}