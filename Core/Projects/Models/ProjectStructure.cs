namespace Requina.Core.Projects.Models;

public class ProjectStructure
{
    public required string EnvironmentDirectory { get; set; }    
    public required string SourceDirectory { get; set; }
    public required ProjectDirectory SourceContent { get; set; }
}

public class ProjectDirectory
{
    public required string Path { get; set; }
    public required ProjectDirectory[] Directories { get; set; }
    public required string[] EndpointFiles { get; set; }
}
