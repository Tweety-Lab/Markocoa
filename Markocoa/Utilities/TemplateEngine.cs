
using Scriban.Runtime;
using Scriban;
using System.Runtime.InteropServices.JavaScript;

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
    public static string Render(string template, object parameters)
    {
        // Parse the template
        var parsedTemplate = Template.Parse(template);

        // Create a Scriban script object to hold custom methods
        var scriptObject = new ScriptObject();

        // Import the parameters
        scriptObject.Import(parameters);

        // Add custom methods
        scriptObject.Import("prettify", new Func<string, string>(Prettify));

        // Create a template context
        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        return parsedTemplate.Render(context);
    }

    /// <summary>
    /// Prettifies the given text.
    /// </summary>
    /// <param name="text">CamelCase text.</param>
    /// <returns>Prettified text.</returns>
    private static string Prettify(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var result = new System.Text.StringBuilder();
        result.Append(text[0]);

        for (int i = 1; i < text.Length; i++)
        {
            char current = text[i];
            char previous = text[i - 1];

            // Add space before uppercase letters that start a new word
            if (char.IsUpper(current) && (char.IsLower(previous) || (i + 1 < text.Length && char.IsLower(text[i + 1]))))
                result.Append(' ');

            result.Append(current);
        }

        return result.ToString();
    }
}
