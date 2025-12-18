namespace Message.Api.Extensions;

internal static class ConfigurationExtensions
{
    public static void AddModuleConfiguration(this IConfigurationBuilder builder, string[] modules)
    {
        foreach (var module in modules)
        {
            builder.AddJsonFile($"modules.{module}.json", optional: true, reloadOnChange: true);
            builder.AddJsonFile($"modules.{module}.Development.json", optional: true, reloadOnChange: true);
        }
    }
}
