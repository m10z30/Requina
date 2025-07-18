using System.Runtime.InteropServices;

namespace Requina.Common.Helpers;

public class FileHelper
{
    public static bool IsAbsolutePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (!Path.IsPathRooted(path))
            return false;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return !string.IsNullOrEmpty(path) && path.Contains(':');
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return path.StartsWith("/");
        }

        return false;
    }
}
