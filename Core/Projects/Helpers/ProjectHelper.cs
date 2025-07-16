using Requina.Common.Constants;
using Requina.Core.Environments.Helpers;
using Requina.Core.Projects.Models;

namespace Requina.Core.Projects.Helpers;

public static class ProjectHelper
{

    public static void PrintProjectStructure(string directory)
    {
        var name = Path.GetFileName(directory);
        var structure = GetProjectStructure(directory);
        Console.WriteLine($"|- {name}");
        PrintEnvironments(structure.EnvironmentDirectory);
        PrintProjectDirectory(structure.SourceContent);
    }

    private static void PrintProjectDirectory(ProjectDirectory projectDirectory, int depth = 0)
    {
        string indent = "|" + string.Concat(Enumerable.Repeat("-", depth + 2));
        string fileIndent = "|" + string.Concat(Enumerable.Repeat("-", depth + 3));
        Console.WriteLine($"{indent} {Path.GetFileName(projectDirectory.Path)}");
        foreach (var file in projectDirectory.EndpointFiles)
        {
            Console.WriteLine($"{fileIndent} {Path.GetFileName(file)}");
        }
        depth += 1;
        foreach (var dir in projectDirectory.Directories)
        {
            PrintProjectDirectory(dir, depth);
        }
    }

    private static void PrintEnvironments(string dir)
    {
        var envs = EnvHelper.GetEnvironments();
        Console.WriteLine($"|-- {Path.GetFileName(dir)}");
        foreach (var env in envs)
        {
            Console.WriteLine($"|--- {env.FileName}");
        }
    }

    public static ProjectStructure GetProjectStructure(string directory)
    {
        var directories = GetDirectories(directory);
        if (!directories.Select(x => Path.GetFileName(x)).Contains(AppConstants.Directories.Source) || !directories.Select(x => Path.GetFileName(x)).Contains(AppConstants.Directories.Environments))
        {
            throw new Exception("project must contain, environements and src directories");
        }
        var sourceDir = directories.Where(x => Path.GetFileName(x) == AppConstants.Directories.Source).First();
        var ProjectStructure = new ProjectStructure
        {
            EnvironmentDirectory = directories.Where(x => Path.GetFileName(x) == AppConstants.Directories.Environments).First(),
            SourceDirectory = sourceDir,
            SourceContent = GetProjectDirectory(sourceDir),
        };
        return ProjectStructure;
    }

    public static ProjectDirectory GetProjectDirectory(string directory)
    {
        var directories = GetDirectories(directory);
        var files = GetEndpointsFiles(directory);
        var projectDirectories = new List<ProjectDirectory>();
        foreach (var dir in directories)
        {
            projectDirectories.Add(GetProjectDirectory(dir));
        }
        return new()
        {
            Path = directory,
            EndpointFiles = files,
            Directories = projectDirectories.ToArray()
        };
    }

    public static string[] GetDirectories(string directory)
    {
        return Directory.GetDirectories(directory);
    }

    public static string[] GetEndpointsFiles(string directory)
    {
        return Directory.GetFiles(directory);
    }
}
