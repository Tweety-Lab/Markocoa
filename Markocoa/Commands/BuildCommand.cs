

using CommandLine;
using Markocoa.Utilities;

namespace Markocoa.Commands;

/// <summary>
/// Command: Build the Markocoa project into a static site.
/// </summary>
[Verb("build", HelpText = "Build the Markocoa project into a static site")]
internal class BuildCommand : ICommand
{
    [Option('p', "path", Required = false, HelpText = "The path to the project directory")]
    public string? Path { get; set; }

    public void Execute()
    {
        // Get .yml files in the project dir
        string projectPath = Path ?? "./";

        string[] files = Directory.GetFiles(projectPath, "*.yml");
        if (files.Length == 0)
        {
            Console.WriteLine($"No project found in {projectPath}.");
            return;
        }

        // Read project settings
        // Assuming the first .yml file is the project settings
        ProjectSettings settings = Serializer.Deserialize<ProjectSettings>(files[0]);

        Compiler.Build(projectPath, settings);
    }
}
