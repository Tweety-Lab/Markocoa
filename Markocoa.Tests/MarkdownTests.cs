using Markocoa.Utilities;

namespace Markocoa.Tests;

public class MarkdownTests
{
    [Fact]
    public void ToHtml_ConvertsBasicMarkdownToHtml()
    {
        string md = "# Title\n\nSome text.";
        string html = Markdown.ToHtml(md);

        // Opening titles can be <h1> or <h1 id="title">
        Assert.Contains("Title</h1>", html);
        Assert.Contains("<p>Some text.</p>", html);
    }

    [Fact]
    public void ToHtml_RewritesMarkdownLinksToHtml()
    {
        string md = "See [Other Page](other-page.md)";
        string html = Markdown.ToHtml(md);

        Assert.DoesNotContain("other-page.md", html);
        Assert.Contains("other-page.html", html);
    }

    [Fact]
    public void ToHtml_RewritesMarkdownLinksWithAnchors()
    {
        string md = "See [Section](other-page.md#part1)";
        string html = Markdown.ToHtml(md);

        Assert.Contains("other-page.html#part1", html);
    }

    [Fact]
    public void ExtractReferencedResources_FindsImageAndFileLinks()
    {
        string md = @"
![Diagram](images/diagram.png)
See [Download](files/manual.pdf)
";

        var resources = Markdown.ExtractReferencedResources(md);

        Assert.Equal(2, resources.Count);
        Assert.Contains(resources, f => f.Name == "diagram.png");
        Assert.Contains(resources, f => f.Name == "manual.pdf");
    }

    [Fact]
    public void ExtractReferencedResources_IgnoresMarkdownLinks()
    {
        string md = "See [Other Page](other-page.md)";
        var resources = Markdown.ExtractReferencedResources(md);

        Assert.Empty(resources);
    }

    [Fact]
    public void ExtractReferencedResources_ReturnsEmptyListForEmptyContent()
    {
        var resources = Markdown.ExtractReferencedResources(string.Empty);
        Assert.Empty(resources);
    }
}
