
using Markdig;

namespace Markocoa.Utilities;

/// <summary>
/// Utility class for building Markocoa projects into static sites.
/// </summary>
internal static class Compiler
{
    /// <summary>
    /// Compiles a Markocoa project into a static site.
    /// </summary>
    /// <param name="projectPath">Path to Markocoa project.</param>
    /// <param name="settings">Settings for the Markocoa project.</param>
    public static void Build(string projectPath, ProjectSettings settings)
    {
        // Create output directory
        string outputPath = Path.Combine(projectPath, "build");
        Directory.CreateDirectory(outputPath);

        // TEMP SOLUTION: Compile all .md files in the project to html in the output directory
        foreach (string file in settings.Files ?? new List<string>())
        {
            string html = Markdown.ToHtml(File.ReadAllText(Path.Combine(projectPath, file)));
            File.WriteAllText(Path.Combine(outputPath, Path.ChangeExtension(Path.GetFileName(file), ".html")), html);
        }
    }
}
