

using CommandLine;
using Markocoa.Utilities;

namespace Markocoa.Commands;

/// <summary>
/// Command: Create a new Markocoa template project.
/// </summary>
[Verb("new", HelpText = "Create a new Markocoa project")]
internal class NewCommand : ICommand
{
    [Option('n', "name", Required = true, HelpText = "The name of the project")]
    public string Name { get; set; } = "MarkocoaProject";

    [Option('p', "path", Required = false, HelpText = "The path to create the project in")]
    public string? Path { get; set; }

    // Default page contents
    private const string PAGE_TEMPLATE = @"
# Markocoa Project
This is a new Markocoa project. You can start editing the `Page.md` file to add your content.
";

    public void Execute()
    {
        // Create project directory
        string projectPath = System.IO.Path.Combine(Path ?? "./", Name);
        Directory.CreateDirectory(projectPath);

        // Create example page
        File.WriteAllText($"{projectPath}Page.md", PAGE_TEMPLATE);

        // Create default options
        ProjectSettings settings = new();
        settings.Name = Name;
        settings.Files = new List<string> { "Page.md" };

        // Serialize project settings
        File.WriteAllText($"{projectPath}{Name}.yml", Serializer.Serialize(settings));
    }
}
