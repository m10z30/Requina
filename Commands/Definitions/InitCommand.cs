using CommandLine;
using Requina.Common.Constants;
using Requina.Helpers.Commands;

namespace Requina.Commands.Definitions;


// Init command options
[Verb("init", HelpText = "Initialize the application")]
public class InitOptions : BaseOptions
{
    [Option('d', "dir", Required = false, HelpText = "Specify the directory of the requina project")]
    public string? Directory { get; set; }
}

public static class InitCommand
{
    public static async Task<int> Execute(InitOptions options)
    {
        AppConstants.VariableConstants.BaseDirectory = string.IsNullOrWhiteSpace(options.Directory) ? Directory.GetCurrentDirectory() : options.Directory;

        if (!Directory.Exists(AppConstants.VariableConstants.BaseDirectory))
        {
            Directory.CreateDirectory(AppConstants.VariableConstants.BaseDirectory);
        }
        else if (Directory.GetDirectories(AppConstants.VariableConstants.BaseDirectory).Length != 0 || Directory.GetFiles(AppConstants.VariableConstants.BaseDirectory).Length != 0)
        {
            throw new Exception($"directory needs to be empty for a project to be created");
        }

        await CreateEnvironmentsAsync();
        await CreateSourceAsync();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("project initialized");

        return 0;
    }

    private static async Task CreateSourceAsync()
    {
        var sourcePath = Path.Join(AppConstants.VariableConstants.BaseDirectory, AppConstants.Directories.Source);
        var authDir = Path.Join(sourcePath, "auth");
        var login = Path.Join(authDir, "login.ren");
        string loginContent = @"# info
name: login
url: {baseUrl}/auth/login
method: Post

# body
{
  ""username"": ""{username}"",
  ""password"": ""{password}""
}";

        var categoriesDir = Path.Join(sourcePath, "categories");
        var addCategory = Path.Join(categoriesDir, "addCategory.ren");
        var addCategoryContent = @"# info
name: addCategory
method: Post
url: {baseUrl}/categories

# headers
Authorization: Bearer {accessToken}

# body
{
  ""name"": ""new category"",
  ""image"": ""something.png""
}
";
        var getCategories = Path.Join(categoriesDir, "getCategories.ren");
        var getCategoriesContent = @"# info
name: getCategories 
url: {baseUrl}/categories
method: Get 

# headers
authorization: Bearer {accessToken}

# query
query: some
";
        Directory.CreateDirectory(sourcePath);
        Directory.CreateDirectory(authDir);
        Directory.CreateDirectory(categoriesDir);
        await File.WriteAllTextAsync(login, loginContent);
        await File.WriteAllTextAsync(getCategories, getCategoriesContent);
        await File.WriteAllTextAsync(addCategory, addCategoryContent);
    }

    private static async Task CreateEnvironmentsAsync()
    {
        var envPath = Path.Join(AppConstants.VariableConstants.BaseDirectory, AppConstants.Directories.Environments);
        var defaultEnv = Path.Join(envPath, "default.env");
        var defaultContent = "!active";
        Directory.CreateDirectory(envPath);
        await File.WriteAllTextAsync(defaultEnv, defaultContent);
    }
}
