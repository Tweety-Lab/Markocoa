using Markocoa.Utilities;

namespace Markocoa.Tests;

public class SerializerTests
{
    private class SampleObject
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool Active { get; set; }
    }

    [Fact]
    public void Serialize_ObjectToYaml_ReturnsValidYaml()
    {
        var obj = new SampleObject
        {
            Name = "Test User",
            Age = 30,
            Active = true
        };

        string yaml = Serializer.Serialize(obj);

        Assert.Contains("Name: Test User", yaml);
        Assert.Contains("Age: 30", yaml);
        Assert.Contains("Active: true", yaml);
    }

    [Fact]
    public void Deserialize_ValidYamlFile_ReturnsExpectedObject()
    {
        var yamlContent =
@"Name: Jane
Age: 25
Active: false";

        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, yamlContent);

        try
        {
            var obj = Serializer.Deserialize<SampleObject>(filePath);

            Assert.Equal("Jane", obj.Name);
            Assert.Equal(25, obj.Age);
            Assert.False(obj.Active);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void Deserialize_InvalidYaml_ThrowsException()
    {
        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, "Invalid: [not valid YAML}");

        try
        {
            Assert.ThrowsAny<Exception>(() =>
                Serializer.Deserialize<SampleObject>(filePath));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void Deserialize_EmptyFile_ReturnsNull()
    {
        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, string.Empty);

        try
        {
            var result = Serializer.Deserialize<SampleObject>(filePath);
            Assert.Null(result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}
