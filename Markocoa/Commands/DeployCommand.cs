using CommandLine;
using LibGit2Sharp;
using Markocoa;
using Markocoa.Utilities;

namespace Markocoa.Commands;

/// <summary>
/// Command: Deploy the Markocoa project to a hosting service.
/// </summary>
[Verb("deploy", HelpText = "Deploy the Markocoa project to a hosting service")]
internal class DeployCommand : ICommand
{
    public enum HostingServiceType { Github };

    [Option('p', "path", Required = false, HelpText = "The path to the project directory")]
    public string? Path { get; set; }

    [Option('s', "service", Required = true, HelpText = "The hosting service to deploy to")]
    public HostingServiceType? HostingService { get; set; }

    public void Execute()
    {
        string projectPath = Path ?? "./";

        if (HostingService == HostingServiceType.Github)
            DeployToGitHub(projectPath);
    }

    static void DeployToGitHub(string projectPath)
    {
        string[] files = Directory.GetFiles(projectPath, "*.yml");
        if (files.Length == 0)
        {
            Console.WriteLine($"No project found in {projectPath}.");
            return;
        }

        // Read project settings
        ProjectSettings settings = Serializer.Deserialize<ProjectSettings>(files[0]);

        // Build the project
        Compiler.Build(projectPath, settings);

        string buildFolder = System.IO.Path.Combine(projectPath, "build");
        if (!Directory.Exists(buildFolder))
        {
            Console.WriteLine("Build folder not found.");
            return;
        }

        Console.WriteLine("Starting deployment to GitHub Pages...");

        // Temporary location for the build folder
        string tempBuildFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "markocoa_build");
        if (Directory.Exists(tempBuildFolder))
            Directory.Delete(tempBuildFolder, true);
        CopyDirectory(buildFolder, tempBuildFolder);

        using var repo = new Repository(projectPath);

        // Save current branch
        var currentBranch = repo.Head.FriendlyName;

        // Clean old files in the branch
        CleanWorkingDirectory(projectPath);

        // Switch to gh-pages branch or create it
        Branch ghPages = repo.Branches["gh-pages"] ?? repo.CreateBranch("gh-pages");

        try
        {
            LibGit2Sharp.Commands.Checkout(repo, ghPages);
        } catch (Exception ex)
        {
            Console.WriteLine($"Failed to checkout gh-pages branch: {ex.Message}! Do you have unsaved changes?");
            return;
        }

        // Copy build files back from temp
        CopyDirectory(tempBuildFolder, projectPath);

        // Deploy to domains
        File.WriteAllText(System.IO.Path.Combine(projectPath, "CNAME"), string.Join("\n", settings.DeployTargets ?? Enumerable.Empty<string>()));

        // Stage all changes
        LibGit2Sharp.Commands.Stage(repo, "*");

        Signature author = new Signature("Markocoa Bot", "bot@markocoa.com", DateTimeOffset.Now);
        repo.Commit("Deploy to GitHub Pages.", author, author);

        // Force push to origin/gh-pages
        var remote = repo.Network.Remotes["origin"];
        string pushRefSpec = $"refs/heads/{ghPages.FriendlyName}:refs/heads/{ghPages.FriendlyName}";

        Git.Execute("push origin gh-pages --force", projectPath);

        // Switch back to original branch
        LibGit2Sharp.Commands.Checkout(repo, currentBranch);

        // Clean up temporary folder
        Directory.Delete(tempBuildFolder, true);

        Console.WriteLine("Deployment completed successfully!");
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dir.Replace(sourceDir, destDir));

        foreach (var file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string dest = file.Replace(sourceDir, destDir);
            File.Copy(file, dest, true);
        }
    }

    private static void CleanWorkingDirectory(string repoPath)
    {
        var files = Directory.GetFiles(repoPath, "*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var name = System.IO.Path.GetFileName(file);
            if (string.Equals(name, "CNAME", StringComparison.OrdinalIgnoreCase))
                continue;
            File.Delete(file);
        }

        var dirs = Directory.GetDirectories(repoPath, "*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dirs)
        {
            var name = System.IO.Path.GetFileName(dir);
            if (string.Equals(name, ".git", StringComparison.OrdinalIgnoreCase))
                continue;
            Directory.Delete(dir, true);
        }
    }
}
