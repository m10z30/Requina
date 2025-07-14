namespace Requina.Commands.Models;

public enum CommandValueType
{
    Text,
    Number,
    None
}

public class CommandDescription
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? NoDashName { get; set; }
    public CommandValueType Type { get; set; }
    public string? Description { get; set; }
}


public class Command
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? NoDashName { get; set; }
    public CommandValueType Type { get; set; }
    public string? Description { get; set; }
    public string? Value { get; set; }
    public int? NumberValue => Type == CommandValueType.Number ? int.Parse(Value!) : throw new Exception($"command '{Name}' type is '{Type}' can't be translated to Number");

}

