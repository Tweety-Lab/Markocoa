
namespace Markocoa.Utilities;

/// <summary>
/// Utility class for handling template rendering.
/// </summary>
internal static class TemplateEngine
{
    /// <summary>
    /// Renders a template with the given parameters.
    /// </summary>
    /// <param name="template">The template string.</param>
    /// <param name="parameters">Object containing template parameters.</param>
    /// <returns>Rendered template string.</returns>
    public static string Render(string template, object parameters) => Scriban.Template.Parse(template).Render(parameters);
}
