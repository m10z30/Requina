using Requina.Common.Constants;
using Requina.Core.Environments.Models;

namespace Requina.Core.Environments.Helpers;

public static class EnvHelper
{

    public static async Task UpdateActiveEnvironmentAsync(string key, string value)
    {
        var env = GetActiveEnvironment();
        var found = false;
        foreach (var envValue in env.Values)
        {
            if (envValue.Name == key)
            {
                found = true;
                envValue.Value = value;
                break;
            }
        }
        if (!found)
        {
            env.Values.Add(new EnvValue
            {
                Name = key,
                Value = value
            });
        }
        await WriteEnvironmentAsync(env);
    }

    private static async Task WriteEnvironmentAsync(Models.Environment env)
    {
        if (!File.Exists(env.FilePath))
        {
            throw new Exception("environment does not exists");
        }
        Console.WriteLine($"is active: {env.IsActive}");
        var newContent = env.IsActive ? "!active\n" : "";
        newContent += string.Join("\n", env.Values.Select(x => $"{x.Name}={x.Value}").ToArray());
        await File.WriteAllTextAsync(env.FilePath, newContent);
    }

    public static Models.Environment GetActiveEnvironment(bool message = true)
    {
        var envs = GetEnvironments();
        var activeEnvs = new List<Models.Environment>();
        foreach (var env in envs)
        {
            if (env.IsActive)
            {
                activeEnvs.Add(env);
            }
        }
        if (activeEnvs.Count == 0)
        {
            var chosen = envs.First();
            if (message)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"no environment is active, the following is chosen as active: {chosen.Name}");
                Console.WriteLine("please make sure the enviroment is active by writing '!active' or '!a' at the start of the file");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"active environment: {chosen.Name}");
                Console.ResetColor();
            }
            return chosen;
        }

        Models.Environment chosenEnv = activeEnvs.FirstOrDefault()!;
        if (activeEnvs.Count > 1)
        {
            var names = activeEnvs.Select(x => x.Name).ToList();
            var count = activeEnvs.Count;
            var msg = string.Join(", ", names);
            chosenEnv = activeEnvs.First();
            if (message)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"mutiple environments are active: [{msg}], '{chosenEnv.Name}' is chosen to be active.");
                Console.WriteLine("please make sure the enviroment is active by writing '!active' or '!a' at the start of the file");
                Console.ResetColor();
            }
        }
        if (message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"active environment: {chosenEnv.Name}");
            Console.ResetColor();
        }
        return chosenEnv;
    }

    public static List<Models.Environment> GetEnvironments()
    {
        if (!Directory.Exists(AppConstants.Environments.EnvironmentDirectory))
        {
            throw new Exception("no 'environments' folder exists, please create one");
        }
        var files = Directory.GetFiles(AppConstants.Environments.EnvironmentDirectory);
        if (files.Length == 0)
        {
            throw new Exception("no environments, please create one at least");
        }
        return files.Select(x => new Models.Environment
        {
            FilePath = x,
            FileName = Path.GetFileName(x),
            Name = Path.GetFileNameWithoutExtension(x),
            IsActive = IsEnvironmentActive(x),
            Values = GetValues(x)
        }).ToList();
    }

    public static bool IsEnvironmentActive(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new Exception($"{filePath} environment file does not exists");
        }
        var lines = File.ReadAllLines(filePath);
        string? firstLine = null; 
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            firstLine = line;
            break;
        }
        if (firstLine == null)
        {
            return false;
        }
        firstLine = firstLine.Trim().ToLower();
        if (firstLine == "!active" || firstLine == "!a")
        {
            return true;
        }
        return false;
    }

    public static List<EnvValue> GetValues(string filePath)
    {
        var result = new List<EnvValue>();
        if (!File.Exists(filePath))
        {
            throw new Exception($"{filePath} environment file does not exists");
        }
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            if (line.StartsWith('#') || line.StartsWith('!') || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            if (!line.Contains('='))
            {
                throw new Exception($"in environment file '{filePath}', in line {line} not currect file format");
            }
            var values = line.Split('=');
            if (values.Length != 2)
            {
                throw new Exception($"in environment file '{filePath}', in line {line} not currect file format");
            }
            result.Add(new()
            {
                Name = values[0].Trim(),
                Value = values[1].Trim(),
            });
        }
        return result;
    }

    public static string RenderContent(string content)
    {
        var activeEnv = GetActiveEnvironment(false);
        foreach (var value in activeEnv.Values)
        {
            content = content.Replace('{' + value.Name + '}', value.Value);
        }
        return content;
    }
}
