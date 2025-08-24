
namespace Markocoa.Themes;

/// <summary>
/// Object representing the theme settings.
/// </summary>
/// <remarks>This class gets serialized to and from a YAML file.</remarks>
internal class ThemeSettings
{
    /// <summary>
    /// The name of the theme.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Path to the page template file.
    /// </summary>
    public string? PageTemplate { get; set; }
}
