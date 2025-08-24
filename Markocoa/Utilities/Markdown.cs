
using System.Text.RegularExpressions;

namespace Markocoa.Utilities;

/// <summary>
/// Utility class for handling Markdown content.
/// </summary>
internal static class Markdown
{
    /// <summary>
    /// Converts Markdown to HTML.
    /// </summary>
    /// <param name="markdown">Markdown content.</param>
    /// <returns>HTML content.</returns>
    public static string ToHtml(string markdown) => Markdig.Markdown.ToHtml(markdown);

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

            if (!string.IsNullOrWhiteSpace(path))
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
