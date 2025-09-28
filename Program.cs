using Requina.Common.Constants;
using Requina.Common.Services;
using Requina.Helpers.Commands;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            return await CommandInitializer.Execute(args);
        }
        catch (Exception ex)
        {
            var errorText = GetErrorText(ex);
            Logger.LogError(errorText);
            return 1;
        }
    }

    static string GetErrorText(Exception ex, int depth = -1)
    {
        var text = "";
        if (depth > -1)
        {
            text += string.Concat(Enumerable.Repeat("    ", depth)) + "└──>";
            if (AppConstants.IsDebug)
            {
                text += ex.Message + " at " + ex.Source + "\n";
            }
            else
            {
                text += ex.Message + "\n";
            }
        }
        else
        {
            if (AppConstants.IsDebug)
            {
                text += ex.Message + " at " + ex.Source + "\n";
            }
            else
            {
                text += ex.Message + "\n";
            }
        }
        if (ex.InnerException is not null)
        {
            text += GetErrorText(ex.InnerException, depth + 1);
        }
        return text;
    }
}

