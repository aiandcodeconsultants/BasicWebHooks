using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BasicWebHooks.Configuration.Tests.Extensions;

/// <summary>
/// A class containing extension methods for <see cref="IConfigurationBuilder"/>.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds settings based on the specified object (serialized to JSON).
    /// </summary>
    /// <param name="builder">The configuration builder to add settings to.</param>
    /// <param name="settingsObject">The object to serialize to JSON to add settings from.</param>
    /// <returns>The configured builder.</returns>
    internal static IConfigurationBuilder AddFromObject(this IConfigurationBuilder builder, object settingsObject)
        => builder
            .AddJsonStream(
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        JsonSerializer.Serialize(
                            settingsObject))));
}
