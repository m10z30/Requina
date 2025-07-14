using System.Collections;
using System.Reflection;

namespace Requina.Common.Extensions;

public static class ObjectExtensions
{
    public static void PrintObject(this object obj, int indent = 0, HashSet<object>? visited = null)
    {
        visited ??= new HashSet<object>();

        if (obj == null)
        {
            Console.WriteLine($"{Indent(indent)}null");
            return;
        }

        Type type = obj.GetType();

        if (!type.IsValueType && visited.Contains(obj))
        {
            Console.WriteLine($"{Indent(indent)}(circular reference to {type.Name})");
            return;
        }

        if (!type.IsValueType)
            visited.Add(obj);

        // Handle simple types
        if (type.IsPrimitive || obj is string || obj is DateTime || obj is decimal)
        {
            string output = obj.ToString()?.Replace("\n", "\\n").Replace("\r", "\\r") ?? "null";
            Console.WriteLine($"{Indent(indent)}{output}");
            return;
        }

        // Handle IEnumerable
        if (obj is IEnumerable enumerable && !(obj is string))
        {
            Console.WriteLine($"{Indent(indent)}[");
            foreach (var item in enumerable)
            {
                item.PrintObject(indent + 2, visited);
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
            value!.PrintObject(indent + 2, visited);
        }

        Console.WriteLine($"{Indent(indent)}}}");
    }

    private static string Indent(int level) => new string(' ', level);
}
