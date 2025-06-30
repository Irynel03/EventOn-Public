using EventOn.API.Client;
using EventOn.BusinessLogic.Helpers;
using EventOn.BusinessLogic.Services;
using EventOn.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventOn;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

        var configService = new ConfigurationService();
        var eventOnApiSettings = configService.GetSettings<EventOnApiSettings>("EventOnApiSettings");

        builder.Services.AddSingleton(Options.Create(eventOnApiSettings));

        // Scoped
        builder.Services.AddScoped<IEventOnApiService, EventOnApiService>();
        builder.Services.AddScoped<IUserDataValidator, UserDataValidator>();
        builder.Services.AddScoped<IOneSignalService, OneSignalService>();

        // Singletons
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<UserData>();
        builder.Services.AddSingleton(Options.Create(eventOnApiSettings));
        builder.Services.AddSingleton<IGeolocationService, GeolocationService>();
        builder.Services.AddSingleton<IThemeHandler, ThemeHandler>();
        builder.Services.AddSingleton<IUserLocalDataService, UserLocalDataService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        ApplyThemeAsync(app.Services).ConfigureAwait(false);

        return app;
    }

    private static async Task ApplyThemeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var themeHandler = scope.ServiceProvider.GetRequiredService<IThemeHandler>();
        await themeHandler.ApplyTheme();
    }
}