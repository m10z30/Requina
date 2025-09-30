using Requina.Common.Constants;
using Requina.Common.Services;
using Requina.Helpers.Commands;

class Program
{
    static async Task<int> Main(string[] args)
    {
        if (AppConstants.IsDebug)
        {
            Logger.LogInfo("running in debug mode"); 
        }
        try
        {
            return await CommandInitializer.Execute(args);
        }
        catch (Exception ex)
        {
            var errorText = GetErrorText(ex);
            if (AppConstants.IsDebug)
            {
                var stackTrace = GetStackTrace(ex);
                errorText += $"\n\n stack trace: \n {stackTrace}";
            }
            Logger.LogError(errorText);
            return 1;
        }
    }

    private static string GetStackTrace(Exception ex, int depth = -1)
    {
        var text = "";
        if (depth == -1)
        {
            text += ex.StackTrace;
        }
        else
        {
            text += "\n---------------\n";
            text += "\nInternal Stack Trace:\n";
            text += ex.StackTrace;
        }
        if (ex.InnerException is not null)
        {
            text += GetStackTrace(ex.InnerException, depth + 1);
        }
        return text;
    }

    static string GetErrorText(Exception ex, int depth = -1)
    {
        var text = "";
        if (depth > -1)
        {
            text += string.Concat(Enumerable.Repeat("    ", depth)) + "└──>";
            text += ex.Message + "\n";
        }
        else
        {
            text += ex.Message + "\n";
        }
        if (ex.InnerException is not null)
        {
            text += GetErrorText(ex.InnerException, depth + 1);
        }
        return text;
    }
}

