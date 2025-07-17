using Requina.Core.Environments.Helpers;
using Requina.Core.Projects.Models;

namespace Requina.Core.Projects.Helpers;

public static class ProjectPrintHelper
{
    public static string GetIndent(int indent, bool more = true)
    {
        if (indent == 0)
        {
            return "|-- ";
        }
        if (more)
        {
            return "|" + string.Concat(Enumerable.Repeat(" ", indent * 3)) + "|-- ";
        }
        return " " + string.Concat(Enumerable.Repeat(" ", indent * 3)) + "|-- ";
    }
    public static void PrintProjectStructure(string directory)
    {
        var name = Path.GetFileName(directory);
        var structure = ProjectHelper.GetProjectStructure(directory);
        Console.WriteLine($"{name}");
        Console.WriteLine("|");
        PrintEnvironments(structure.EnvironmentDirectory);
        PrintProjectDirectory(structure.SourceContent, 0, false);
    }

    private static void PrintProjectDirectory(ProjectDirectory projectDirectory, int depth, bool more)
    {
        var moreDirectories = projectDirectory.Directories.Length > 0;
        Console.WriteLine($"{GetIndent(depth, more)}{Path.GetFileName(projectDirectory.Path)}");
        foreach (var file in projectDirectory.EndpointFiles)
        {
            Console.WriteLine($"{GetIndent(depth + 1, more)}{Path.GetFileName(file)}");
        }
        depth += 1;
        foreach (var dir in projectDirectory.Directories)
        {
            PrintProjectDirectory(dir, depth, moreDirectories);
        }
    }

    private static void PrintEnvironments(string dir)
    {
        var envs = EnvHelper.GetEnvironments();
        Console.WriteLine($"{GetIndent(0)}{Path.GetFileName(dir)}");
        foreach (var env in envs)
        {
            Console.WriteLine($"{GetIndent(1)}{env.FileName}");
        }
    }
}
