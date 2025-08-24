
using Markdig;
using Markocoa.Themes;

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
        // Check if theme exists
        string themePath = Themes.Themes.GetThemePath(settings.Theme ?? "Default")!;
        if (themePath == null)
        {
            Console.WriteLine($"Theme '{settings.Theme}' not found! Using default theme.");
            settings.Theme = "Default";
            themePath = Themes.Themes.GetThemePath("Default")!;
        }

        // Load theme
        ThemeSettings themeSettings = Serializer.Deserialize<ThemeSettings>(themePath);

        // Create output directory
        string outputPath = Path.Combine(projectPath, "build");
        Directory.CreateDirectory(outputPath);

        // Compile all .md files in the project to html in the output directory
        foreach (string file in settings.Files ?? new List<string>())
        {
            string template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(themePath) ?? "./", themeSettings.PageTemplate));
            string markdownHTML = Markdown.ToHtml(File.ReadAllText(Path.Combine(projectPath, file)));

            // Render template
            string html = TemplateEngine.Render(template, new { PageContent = markdownHTML });


            File.WriteAllText(Path.Combine(outputPath, Path.ChangeExtension(Path.GetFileName(file), ".html")), html);
        }
    }
}
