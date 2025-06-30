using EventOn.API.Models.Enums;

namespace EventOn.Interfaces;

public interface IThemeHandler
{
    Task ApplyTheme();
    Task SaveThemeLocally(Theme theme);
}