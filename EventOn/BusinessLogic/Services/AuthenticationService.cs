using EventOn.API.Models;
using EventOn.API.Models.Enums;
using EventOn.BusinessLogic.Helpers;
using EventOn.Interfaces;
using OneSignalSDK.DotNet;

namespace EventOn.BusinessLogic.Services;

public class AuthenticationService(IEventOnApiService eventOnApiService,
                            IOneSignalService oneSignalService,
                            IUserLocalDataService userLocalDataService,
                            IThemeHandler themeHandler) : IAuthenticationService
{
    private readonly IEventOnApiService _eventOnApiService = eventOnApiService;
    private readonly IOneSignalService _oneSignalService = oneSignalService;
    private readonly IThemeHandler _themeHandler = themeHandler;

    private const string UsernameKey = "username";
    private const string PasswordKey = "password";
    private const string JWTKey = "jwt_token";

    public async Task<Result> Authenticate(UserLoginRequestData userData)
    {
        var result = new Result();

        try
        {
            var loginResponseResult = await _eventOnApiService.LoginUser(userData);
            if (loginResponseResult.HasErrors)
            {
                result.Concat(loginResponseResult);
                return result;
            }

            await SaveCredentialsAsync(userData.Username, userData.Password, loginResponseResult.Data.Token);

            var getPreferencesResult = await _eventOnApiService.GetUserPreferences(userData.Username);
            if (getPreferencesResult.HasErrors)
            {
                result.Concat(getPreferencesResult);
                return result;
            }

            await _themeHandler.SaveThemeLocally(getPreferencesResult.Data.SelectedTheme);

            var hasCreatedEvents = (await _eventOnApiService.GetAllEventsFromUser(userData.Username)).Data.Count > 0;
            var getUserResult = await _eventOnApiService.GetUser(userData.Username);
            if (getUserResult.HasErrors)
            {
                result.Concat(getUserResult);
                return result;
            }

            UserData.Instance.SetNewUser(getUserResult.Data, getPreferencesResult.Data, hasCreatedEvents);
            await _themeHandler.ApplyTheme();

            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
                OneSignal.Login(getUserResult.Data.Username);

            var playerIds = await _oneSignalService.GetPlayerIdsByExternalIdAsync(getUserResult.Data.Username);
            foreach (var playerId in playerIds)
            {
                if (!getUserResult.Data.OneSignalPlayerIds.Contains(playerId))
                {
                    var updateUserResult = await _eventOnApiService.AddUserOneSignalId(getUserResult.Data.Username, playerId);
                    if (updateUserResult.HasErrors)
                    {
                        result.Concat(updateUserResult);
                        return result;
                    }
                }
            }

            var getOrdersResult = await _eventOnApiService.GetAllOrdersForUser(UserData.Instance.Username);
            if (!getOrdersResult.HasErrors)
            {
                userLocalDataService.ClearOrders();
                userLocalDataService.SaveOrders(getOrdersResult.Data);
            }

        }
        catch
        {
            result.AddError("Can't authenticate");
        }

        return result;
    }

    public async Task<Result> CreateNewUser(string username, string password, string email, TypeOfUser type)
    {
        var result = new Result();

        if (ValidateNewUserData(username, password, email))
        {
            var newUser = new User()
            {
                Username = username,
                Password = password,
                Email = email,
                UserType = type
            };

            result.Concat(await _eventOnApiService.CreateUser(newUser));
        }

        return result;
    }

    private static bool ValidateNewUserData(string username, string password, string email)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            return false;

        return true;
    }

    public async Task SaveCredentialsAsync(string username, string password, string token)
    {
        await SecureStorage.SetAsync(UsernameKey, username);
        await SecureStorage.SetAsync(PasswordKey, password);
        await SecureStorage.SetAsync(JWTKey, token);
    }

    public async Task SaveCredentialsAsync(string username, string password)
    {
        await SecureStorage.SetAsync(UsernameKey, username);
        await SecureStorage.SetAsync(PasswordKey, password);
    }

    public async Task<Result<User>> LoadSavedCredentialsAsync()
    {
        var result = new Result<User>(new());

        try
        {
            var username = await SecureStorage.GetAsync(UsernameKey);
            var password = await SecureStorage.GetAsync(PasswordKey);

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                result.Data = new User()
                {
                    Username = username,
                    Password = password
                };
            }
        }
        catch (Exception ex)
        {
            result.AddError(ex.Message);
        }

        return result;
    }
}