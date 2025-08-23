
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
    /// List of markdown files in the project.
    /// </summary>
    public List<string>? Files { get; set; }
}
