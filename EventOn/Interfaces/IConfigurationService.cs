using Microsoft.Extensions.Configuration;

namespace EventOn.Interfaces;

public interface IConfigurationService
{
    IConfiguration LoadConfiguration();
    T GetSettings<T>(string sectionName) where T : class, new();
}