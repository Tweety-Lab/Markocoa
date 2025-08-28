using Markocoa.Utilities;
using Xunit;

namespace Markocoa.Tests;

public class TemplateEngineTests
{
    private class SampleParameters
    {
        public string Name { get; set; } = "World";
        public string Greeting { get; set; } = "Hello";
    }

    [Fact]
    public void Render_ValidTemplate_ReplacesVariables()
    {
        // Arrange
        var template = "{{ greeting }}, {{ name }}!";
        var parameters = new SampleParameters();

        // Act
        var result = TemplateEngine.Render(template, parameters);

        // Assert
        Assert.Contains("Hello", result);
        Assert.Contains("World", result);
        Assert.Equal("Hello, World!", result.Trim());
    }

    [Fact]
    public void Render_UsesPrettifyMethod()
    {
        // Arrange
        var template = "{{ prettify 'CamelCaseTest' }}";

        // Act
        var result = TemplateEngine.Render(template, new { });

        // Assert
        Assert.Equal("Camel Case Test", result.Trim());
    }

    [Fact]
    public void Render_EmptyTemplate_ReturnsEmptyString()
    {
        // Arrange
        var template = string.Empty;
        var parameters = new SampleParameters();

        // Act
        var result = TemplateEngine.Render(template, parameters);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Render_MissingParameter_RendersEmptyPlaceholder()
    {
        // Arrange
        var template = "Hello, {{ missing_name }}!";
        var parameters = new { }; // no properties

        // Act
        var result = TemplateEngine.Render(template, parameters);

        // Assert
        // Scriban leaves placeholders empty if not found
        Assert.Equal("Hello, !", result.Trim());
    }

    [Fact]
    public void Prettify_NullOrWhitespace_ReturnsSame()
    {
        // Arrange
        var template = "{{ prettify '' }}"; // empty string

        // Act
        var result = TemplateEngine.Render(template, new { });

        // Assert
        Assert.Equal(string.Empty, result);

        // And with null
        template = "{{ prettify nil }}";
        result = TemplateEngine.Render(template, new { });
        Assert.Equal(string.Empty, result);
    }
}
