using Markocoa.Hosting;
using CommandLine;
using Markocoa.Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        // Open localhost in the default browser
        OpenBrowser("http://localhost:8080");

        // Lock the thread
        while (true)
            Thread.Sleep(1000);
    }

    /// <summary>
    /// Open the default web browser to the specified URL.
    /// </summary>
    /// <param name="url">URL to open.</param>
    static void OpenBrowser(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open browser: {ex.Message}");
        }
    }
}
