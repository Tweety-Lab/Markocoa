namespace Markocoa.Utilities;

/// <summary>
/// Utility class for serializing and deserializing objects.
/// </summary>
internal static class Serializer
{
    /// <summary>
    /// Serializes an object to a YAML string.
    /// </summary>
    /// <typeparam name="T">Type of object to serialize.</typeparam>
    /// <param name="obj">Object to serialize.</param>
    /// <returns>YAML string.</returns>
    public static string Serialize<T>(T obj)
    {
        var serializer = new YamlDotNet.Serialization.Serializer();
        return serializer.Serialize(obj);
    }

    /// <summary>
    /// Deserializes a YAML File to an object.
    /// </summary>
    /// <typeparam name="T">Type of object to deserialize.</typeparam>
    /// <param name="filePath">Path to YAML file.</param>
    /// <returns>Deserialized object.</returns>
    public static T Deserialize<T>(string filePath)
    {
        var deserializer = new YamlDotNet.Serialization.Deserializer();
        return deserializer.Deserialize<T>(File.ReadAllText(filePath));
    }
}
