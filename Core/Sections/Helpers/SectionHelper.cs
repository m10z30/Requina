using System.Text.RegularExpressions;
using Requina.Core.Sections.Models;

namespace Requina.Core.Sections.Helpers;

public static class SectionHelper
{
    public static List<Section> GetSections(string content)
    {
        // Match headers and content using regex
        var matches = Regex.Matches(content, @"^# (.+)\n((?:.+\n?)*)", RegexOptions.Multiline);

        var sections = new List<Section>();

        foreach (Match match in matches)
        {
            var sectionName = match.Groups[1].Value.Trim();
            var sectionContent = match.Groups[2].Value.Trim();
            var section = new Section()
            {
                Name = sectionName,
                Content = sectionContent
            };
            sections.Add(section);
        }
        return sections;
    }

    public static List<SectionParameter> GetParameters(string content)
    {
        var parameters = new List<SectionParameter>();
        var lines = content.Split("\n");
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            if (!line.Contains(':'))
            {
                throw new Exception($"parameter should have a ':' delimiter, the following line: {line}");
            }
            var variables = line.Split(":");
            if (variables.Length < 2)
            {
                throw new Exception($"the format for a parameter should be: name: value, the following line: {line}");
            }
            var name = variables[0];
            var value = string.Join(":", variables.Where(x => x != name));
            var parameter = new SectionParameter
            {
                Name = name,
                Content = value
            };
            parameters.Add(parameter);
        }
        return parameters;
    }
}
