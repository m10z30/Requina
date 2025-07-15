using Requina.Core.Environments.Helpers;

namespace Requina.Core.Environments.Models;

public class Environment
{
    public required string FilePath { get; set; }
    public string FileName => Path.GetFileName(FilePath);
    public string Name => Path.GetFileNameWithoutExtension(FilePath);
    public Dictionary<string, string> Values => EnvHelper.GetValues(FilePath);
}
