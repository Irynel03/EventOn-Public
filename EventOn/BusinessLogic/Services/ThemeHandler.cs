using EventOn.Interfaces;
using EventOn.BusinessLogic.Helpers;
using EventOn.API.Models.Enums;

namespace EventOn.BusinessLogic.Services;

public class ThemeHandler : IThemeHandler
{
    private const string ThemeKey = "theme_key";

    public async Task ApplyTheme()
    {
        try
        {
            var themeString = await SecureStorage.GetAsync(ThemeKey);

            if (!string.IsNullOrEmpty(themeString) && Enum.TryParse(themeString, out Theme theme))
                UserData.Instance.Preferences.SelectedTheme = theme;
            else
            {
                UserData.Instance.Preferences.SelectedTheme = Theme.Light;
                Console.WriteLine("Theme is not set or invalid. Using default theme.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying theme: {ex.Message}");
        }
    }

    public async Task SaveThemeLocally(Theme theme)
    {
        await SecureStorage.SetAsync(ThemeKey, theme.ToString());
    }
}