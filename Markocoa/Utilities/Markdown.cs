
using System.Text.RegularExpressions;
using Markdig;

namespace Markocoa.Utilities;

/// <summary>
/// Utility class for handling Markdown content.
/// </summary>
public static class Markdown
{
    /// <summary>
    /// Converts Markdown to HTML.
    /// </summary>
    /// <param name="markdown">Markdown content.</param>
    /// <returns>HTML content.</returns>
    public static string ToHtml(string markdown)
    {
        // Regex to replace links to other Markdown files with .html
        string updatedMarkdown = Regex.Replace(markdown,
            @"\[(.*?)\]\((.*?)\.md(\#.*?)?\)",
            match =>
            {
                string text = match.Groups[1].Value;
                string path = match.Groups[2].Value;
                string fragment = match.Groups[3].Value; // optional #section
                string htmlPath = path + ".html" + fragment;
                return $"[{text}]({htmlPath})";
            },
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Create a Markdown pipeline
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();


        // Convert Markdown to HTML
        return Markdig.Markdown.ToHtml(updatedMarkdown, pipeline);
    }

    /// <summary>
    /// Extracts referenced resources from Markdown content.
    /// </summary>
    /// <param name="markdown">Markdown content.</param>
    /// <returns>List of referenced resources.</returns>
    public static List<FileInfo> ExtractReferencedResources(string markdown)
    {
        var resources = new List<FileInfo>();

        if (string.IsNullOrWhiteSpace(markdown))
            return resources;

        // Regex for Markdown links/images: ![Alt](path) or [Text](path)
        var regex = new Regex(@"!\[.*?\]\((.*?)\)|\[(?:.*?)\]\((.*?)\)", RegexOptions.Compiled);

        var matches = regex.Matches(markdown);
        foreach (Match match in matches)
        {
            // Group 1 is for images, Group 2 is for links
            string path = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;

            // Ignore invalid or markdown files
            if (!string.IsNullOrWhiteSpace(path) && !path.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    resources.Add(new FileInfo(path));
                }
                catch
                {
                    // Ignore invalid paths
                }
            }
        }

        return resources;
    }
}
