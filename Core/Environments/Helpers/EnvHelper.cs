using Requina.Common.Constants;

namespace Requina.Core.Environments.Helpers;

public static class EnvHelper
{

    public static List<Models.Environment> GetEnvironments()
    {
        if (Directory.Exists(AppConstants.Environments.EnvironmentDirectory))
        {
            Directory.CreateDirectory(AppConstants.Environments.EnvironmentDirectory);
            File.Create(Path.Join(AppConstants.Environments.EnvironmentDirectory, "default.env"));
        }
        var files = Directory.GetFiles(AppConstants.Environments.EnvironmentDirectory);
        return files.Select(x => new Models.Environment{ FilePath=x }).ToList();
    }

    public static Dictionary<string, string> GetValues(string filePath)
    {
        var result = new Dictionary<string, string>();
        if (!File.Exists(filePath))
        {
            throw new Exception($"{filePath} environment file does not exists");
        }
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            if (line.StartsWith('#'))
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
            result.Add(values[0].Trim(), values[1].Trim());
        }
        return result;
    }
}
