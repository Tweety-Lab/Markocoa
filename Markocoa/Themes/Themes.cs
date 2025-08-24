
using Markocoa.Utilities;

namespace Markocoa.Themes;

/// <summary>
/// Utility class for managing themes in Markocoa.
/// </summary>
internal static class Themes
{
    /// <summary>
    /// Finds the path to a theme file.
    /// </summary>
    /// <param name="themeName"></param>
    /// <returns>Path to the theme file or null if not found.</returns>
    public static string? GetThemePath(string themeName)
    {
        // First try to find it from our working directory
        string rootDir = Directory.GetCurrentDirectory();
        string[] themeFiles = Directory.GetFiles(rootDir, "*.yml", SearchOption.AllDirectories);
        foreach (string file in themeFiles)
        {
            // Deserialize the YAML file to check the theme name
            var settings = Serializer.Deserialize<ThemeSettings>(File.ReadAllText(file));
            if (settings.Name == themeName)
                return Path.GetDirectoryName(file) ?? string.Empty;
        }

        // If not found, check the execution directory
        string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        themeFiles = Directory.GetFiles(exeDir, "*.yml", SearchOption.AllDirectories);
        foreach (string file in themeFiles)
        {
            // Deserialize the YAML file to check the theme name
            var settings = Serializer.Deserialize<ThemeSettings>(File.ReadAllText(file));
            if (settings.Name == themeName)
                return Path.GetDirectoryName(file) ?? string.Empty;
        }

        return null;
    }
}
