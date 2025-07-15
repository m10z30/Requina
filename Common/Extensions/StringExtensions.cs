namespace Requina.Common.Extensions;

public static class StringExtensions
{
    public static string AddLine(this string str, string value)
    {
        str += "\n";
        str += value;
        return str;
    }
}
