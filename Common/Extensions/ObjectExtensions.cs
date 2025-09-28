using System.Collections;
using System.Reflection;

namespace Requina.Common.Extensions;

public static class ObjectExtensions
{
    public static void PrintObject(this object obj, int indent = 0, int depth = 0, int maxDepth = 5, HashSet<object>? visited = null)
    {
        visited ??= [];

        if (obj == null)
        {
            Console.WriteLine($"null");
            return;
        }

        if (depth > maxDepth)
        {
            Console.WriteLine($"{Indent(indent)}... (max depth reached)");
            return;
        }

        Type type = obj.GetType();

        if (!type.IsValueType && visited.Contains(obj))
        {
            Console.WriteLine($"{obj}  (circular reference)");
            return;
        }

        if (!type.IsValueType)
            visited.Add(obj);

        // Handle simple types
        if (type.IsPrimitive || obj is string || obj is DateTime || obj is decimal)
        {
            string output = obj.ToString()?.Replace("\n", "\\n").Replace("\r", "\\r") ?? "null";
            Console.WriteLine($"{output}");
            return;
        }

        if (type.IsEnum)
        {
            Console.WriteLine($"{obj} (enum)");
            return;
        }

        // Handle IEnumerable
        if (obj is IEnumerable enumerable && !(obj is string))
        {
            Console.WriteLine($"[");
            foreach (var item in enumerable)
            {
                item.PrintObject(indent + 2, depth + 1, maxDepth, visited);
            }
            Console.WriteLine($"{Indent(indent)}]");
            return;
        }

        // Handle complex object
        Console.WriteLine($"{Indent(indent)}{type.Name}");
        Console.WriteLine($"{Indent(indent)}{{");

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value;
            try
            {
                value = prop.GetValue(obj, null);
            }
            catch
            {
                value = "(unavailable)";
            }

            Console.Write($"{Indent(indent + 2)}{prop.Name} = ");
            value!.PrintObject(indent + 2, depth + 1, maxDepth, visited);
        }

        Console.WriteLine($"{Indent(indent)}}}");
    }

    private static string Indent(int level) => new string(' ', level * 2);
}
