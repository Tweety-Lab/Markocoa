using Markocoa.Hosting;
using CommandLine;
using Markocoa.Utilities;

namespace Markocoa.Commands;

/// <summary>
/// Command: Host the Markocoa project locally.
/// </summary>
[Verb("host", HelpText = "Locally host a Markocoa project.")]
internal class HostCommand : ICommand
{
    [Option('p', "path", Required = false, HelpText = "The path to the project directory")]
    public string? Path { get; set; }

    public void Execute()
    {
        // Build the site
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

        // Start the server
        WebHost server = new WebHost(System.IO.Path.Combine(Path ?? "./", "build"), 8080);
        server.Refresh();

        // Watch Markdown files in the project
        var watcher = new FileSystemWatcher(projectPath, "*.md")
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        watcher.Changed += (s, e) =>
        {
            Console.WriteLine($"Detected change in {e.FullPath}, rebuilding...");
            Compiler.Build(projectPath, settings);
            server.Refresh();
        };

        watcher.Created += (s, e) =>
        {
            Console.WriteLine($"Detected new file {e.FullPath}, rebuilding...");
            Compiler.Build(projectPath, settings);
            server.Refresh();
        };

        watcher.Renamed += (s, e) =>
        {
            Console.WriteLine($"Detected rename {e.FullPath}, rebuilding...");
            Compiler.Build(projectPath, settings);
            server.Refresh();
        };

        watcher.Deleted += (s, e) =>
        {
            Console.WriteLine($"Detected deletion {e.FullPath}, rebuilding...");
            Compiler.Build(projectPath, settings);
            server.Refresh();
        };

        Console.WriteLine("Hosting project with live reload. Press Ctrl+C to exit.");

        // Lock the thread
        while (true)
            Thread.Sleep(1000);
    }
}
