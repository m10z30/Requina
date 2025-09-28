using Requina.Common.Constants;
using Requina.Core.Endpoints.Models;
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
        var files = Directory.GetFiles(directory)
            .Where(x => Path.GetExtension(x).TrimStart('.') == AppConstants.Endpoints.FileExtension)
            .ToArray();
        return files;
    }

    public static List<EndpointEvent> GetGlobalEndpointEvents()
    {
        var events = new List<EndpointEvent>();
        var beforeFileName = Path.Join(AppConstants.VariableConstants.BaseDirectory, AppConstants.Directories.Source, "before.rev");
        var afterFileName = Path.Join(AppConstants.VariableConstants.BaseDirectory, AppConstants.Directories.Source, "after.rev");
        if (File.Exists(beforeFileName))
        {
            var content = File.ReadAllText(beforeFileName);
            var beforeEvent = new EndpointEvent
            {
                FilePath = beforeFileName,
                Type = EndpointEventType.Before,
                Lines = content.Split("\n").ToList(),
            };
            events.Add(beforeEvent);
        }
        if (File.Exists(afterFileName))
        {
            var content = File.ReadAllText(afterFileName);
            var afterEvent = new EndpointEvent
            {
                FilePath = afterFileName,
                Type = EndpointEventType.After,
                Lines = content.Split("\n").ToList(),
            };
            events.Add(afterEvent);
        }
        return events;
    }
}
