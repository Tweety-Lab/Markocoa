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

        // Flatten list of all files for navigation context
        var pagesForSidebar = new List<SidebarCategory>();
        foreach (var category in settings.Categories ?? new List<Category>())
        {
            var pageList = category.Files.Select(f => new Page
            {
                Name = Path.GetFileNameWithoutExtension(f),
                Path = "/" + Path.Combine(category.CategoryName, Path.ChangeExtension(Path.GetFileName(f), ".html"))
                             .Replace("\\", "/") // root-relative URL for navigation
            }).ToList();

            pagesForSidebar.Add(new SidebarCategory
            {
                CategoryName = category.CategoryName,
                Pages = pageList
            });
        }

        // Compile all markdown files
        foreach (var category in settings.Categories ?? new List<Category>())
        {
            // Make a folder for this category
            string categoryOutputPath = Path.Combine(outputPath, category.CategoryName);
            Directory.CreateDirectory(categoryOutputPath);

            foreach (string file in category.Files)
            {
                string template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(themePath) ?? "./", themeSettings.PageTemplate));
                string markdownHTML = Markdown.ToHtml(File.ReadAllText(Path.Combine(projectPath, file)));

                // Write template
                object context = new
                {
                    PageContent = markdownHTML,
                    PageTitle = Path.GetFileNameWithoutExtension(file),
                    PageCategoryTitle = category.CategoryName,
                    Categories = pagesForSidebar
                };

                string html = TemplateEngine.Render(template, context);
                string outputFilePath = Path.Combine(categoryOutputPath, Path.ChangeExtension(Path.GetFileName(file), ".html"));
                File.WriteAllText(outputFilePath, html);

                // Copy resources referenced in the markdown
                List<FileInfo> resources = Markdown.ExtractReferencedResources(File.ReadAllText(Path.Combine(projectPath, file)));
                foreach (FileInfo resource in resources)
                {
                    string destResourcePath = Path.Combine(categoryOutputPath, resource.Name);
                    File.Copy(Path.Combine(projectPath, resource.Name), destResourcePath, true);
                }
            }
        }
    }
}

// A Page in a template context
class Page
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}

// A Category for the sidebar in a template context
class SidebarCategory
{
    public string CategoryName { get; set; } = string.Empty;
    public List<Page> Pages { get; set; } = new List<Page>();
}