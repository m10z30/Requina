using Requina.Core.Sections.Helpers;

namespace Requina.Core.Sections.Models;

public class Section
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<SectionParameter> Parameters => SectionHelper.GetParameters(Content);
}

public class SectionParameter
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
