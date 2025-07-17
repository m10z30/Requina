using Requina.Core.Environments.Helpers;
using Requina.Core.Projects.Models;

namespace Requina.Core.Projects.Helpers;

public static class ProjectPrintHelper
{
    public static void PrintProjectStructure(string directory)
    {
        var name = Path.GetFileName(directory);
        var structure = ProjectHelper.GetProjectStructure(directory);
        Console.WriteLine(name);
        PrintEnvironments(structure.EnvironmentDirectory, "├── ");
        PrintProjectDirectory(structure.SourceContent, "", true);
    }

    private static void PrintProjectDirectory(ProjectDirectory projectDirectory, string indent, bool isLast)
    {
        var branch = isLast ? "└── " : "├── ";
        Console.WriteLine($"{indent}{branch}{Path.GetFileName(projectDirectory.Path)}");

        var subIndent = indent + (isLast ? "    " : "│   ");

        var files = projectDirectory.EndpointFiles;
        for (int i = 0; i < files.Length; i++)
        {
            var fileBranch = (i == files.Length - 1 && projectDirectory.Directories.Length == 0) ? "└── " : "├── ";
            Console.WriteLine($"{subIndent}{fileBranch}{Path.GetFileName(files[i])}");
        }

        for (int i = 0; i < projectDirectory.Directories.Length; i++)
        {
            var dir = projectDirectory.Directories[i];
            bool isDirLast = i == projectDirectory.Directories.Length - 1;
            PrintProjectDirectory(dir, subIndent, isDirLast);
        }
    }

    private static void PrintEnvironments(string dir, string prefix)
    {
        var envs = EnvHelper.GetEnvironments();
        Console.WriteLine($"{prefix}{Path.GetFileName(dir)}");

        for (int i = 0; i < envs.Count; i++)
        {
            var isLast = i == envs.Count - 1;
            var branch = isLast ? "└── " : "├── ";
            var indent = prefix.Replace("── ", "   ").Replace("├", "│");
            Console.WriteLine($"{indent}{branch}{envs[i].FileName}");
        }
    }
}

