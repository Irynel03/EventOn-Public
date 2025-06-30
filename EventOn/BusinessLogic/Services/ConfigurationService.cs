using EventOn.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace EventOn.BusinessLogic.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService()
    {
        _configuration = LoadConfiguration();
    }

    public IConfiguration LoadConfiguration()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "EventOn.appsettings.json";

        using Stream stream = assembly.GetManifestResourceStream(resourceName) ??
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        using StreamReader reader = new(stream);
        string json = reader.ReadToEnd();

        return new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();
    }

    public T GetSettings<T>(string sectionName) where T : class, new()
    {
        var settings = new T();
        _configuration.GetSection(sectionName).Bind(settings);
        return settings;
    }
}