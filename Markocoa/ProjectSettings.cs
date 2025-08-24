
namespace Markocoa;

/// <summary>
/// Object representing the project settings.
/// </summary>
/// <remarks>This class gets serialized to and from a YAML file.</remarks>
internal class ProjectSettings
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The theme of the project.
    /// </summary>
    public string? Theme { get; set; } = "Default";

    /// <summary>
    /// List of categories containing files.
    /// </summary>
    public List<Category>? Categories { get; set; }
}

/// <summary>
/// Represents a category with a list of files.
/// </summary>
internal class Category
{
    /// <summary>
    /// The name of the category.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// The list of files under this category.
    /// </summary>
    public List<string> Files { get; set; } = new List<string>();
}
