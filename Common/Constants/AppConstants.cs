namespace Requina.Common.Constants;

public static class AppConstants
{
    #if DEBUG
    public static bool IsDebug = true; 
    #else
    public static bool IsDebug = false; 
    #endif
    public static class Directories
    {
        public static readonly string Source = "src";
        public static readonly string Environments = "environments";
    }

    public static class VariableConstants
    {
        public static string BaseDirectory { get; set; } = Directory.GetCurrentDirectory();
    }

    public static class Environments
    {
        public static readonly string EnvironmentDirectory = Path.Join(VariableConstants.BaseDirectory, "environments"); 
    }

    public static class Endpoints
    {
        public static readonly string FileExtension = "ren";
    }

    public static class Sections
    {
        public static class Info
        {
            public static readonly string Name = "info";
            public static class Parameters
            {
                public static readonly string EndpointName = "name";
                public static readonly string Url = "url";
                public static readonly string Method = "method";
            }
        }
        public static class Headers
        {
            public static readonly string Name = "headers";
        }

        public static class Query
        {
            public static readonly string Name = "query";
        }

        public static class Cookies
        {
            public static readonly string Name = "cookies";
        }

        public static class Body
        {
            public static readonly string JsonName = "body";
            public static readonly string FormName = "form";
            public static readonly string XFormName = "xform";
        }
    }
}
