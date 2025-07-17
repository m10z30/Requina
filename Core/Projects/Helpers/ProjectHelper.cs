using Requina.Common.Constants;
using Requina.Core.Projects.Models;

namespace Requina.Core.Projects.Helpers;

public static class ProjectHelper
{
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
