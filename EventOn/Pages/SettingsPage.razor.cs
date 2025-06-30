using EventOn.BusinessLogic.Helpers;
using EventOn.Components;
using EventOn.Interfaces;
using EventOn.API.Models;
using Microsoft.AspNetCore.Components;
using OneSignalSDK.DotNet;
using Theme = EventOn.API.Models.Enums.Theme;

namespace EventOn.Pages;

public partial class SettingsPage
{
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }
    [Inject]
    private IThemeHandler _themeHandler { get; set; }
    [Inject]
    private IAuthenticationService _authenticationService { get; set; }
    [Inject]
    private IUserDataValidator _userDataValidator { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }

    private Toast toast;

    private const string UsernameKey = "username";
    private const string PasswordKey = "password";

    private bool _showChangePasswordPromt;
    private bool _showDeleteAccountPromt;
    private bool _showChangeAccountPromt;
    private bool _showCardInfoPromt;
    private bool _showAddMoneyPrompt;
    private bool _showNotificationsPrompt;

    private string newPassword = string.Empty;
    private string repeatedPassword = string.Empty;
    private double? _amountToAdd = 0;
    private NotificationPreferences _notificationPreferences = new();
    private double? AmountToAdd
    {
        get => _amountToAdd;
        set => _amountToAdd = (value.HasValue && value.Value < 0) ? 0 : value;
    }

    private string ThemeButtonText =>
       UserData.Instance.Preferences.SelectedTheme == Theme.Dark
           ? "Switch to Light Mode"
           : "Switch to Dark Mode";
    private bool IsPromptOpened => !(_showChangePasswordPromt || _showDeleteAccountPromt ||
        _showChangeAccountPromt || _showCardInfoPromt || _showNotificationsPrompt);

    private void ChangeAccount()
    {
        UserData.Instance.Reset();
        SecureStorage.Remove(PasswordKey);
        SecureStorage.Remove(UsernameKey);

        if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            OneSignal.Logout();

        Navigation.NavigateTo("/");
    }

    private void OpenChangePasswordPrompt()
    {
        _showChangePasswordPromt = true;
    }

    private async Task ChangePassword()
    {
        var passwordsVerification = _userDataValidator.ArePasswordsValid(newPassword, repeatedPassword);
        if (passwordsVerification.HasErrors)
        {
            toast.ShowToast(passwordsVerification.ErrorMessages);
            return;
        }

        var updateResult = await _eventOnApiService.UpdateUserPassword(UserData.Instance.Username, newPassword);
        if (updateResult.HasErrors)
        {
            toast.ShowToast(updateResult.ErrorMessages);
            return;
        }

        toast.ShowToast("Password changed succesfully", false);
        await _authenticationService.SaveCredentialsAsync(UserData.Instance.Username, newPassword);
        _showChangePasswordPromt = false;
    }

    private async Task CreateCard()
    {
        var result = await _eventOnApiService.CreateCard(UserData.Instance.Username);
        if (result.HasErrors)
        {
            toast.ShowToast(result.ToString());
            return;
        }
        else
            toast.ShowToast("Card created", false);

        UserData.Instance.HasCard = true;
    }

    private async Task AddMoney()
    {
        if (_amountToAdd == null || _amountToAdd <= 0)
        {
            toast.ShowToast("Please enter a valid amount.");
            return;
        }

        if (_amountToAdd >= 100000)
        {
            toast.ShowToast("The amount can't be bigger than 100.000.");
            return;
        }

        var addMoneyToUserResult = await _eventOnApiService.AddMoneyToUser(UserData.Instance.Username, (decimal)_amountToAdd);
        if (addMoneyToUserResult.HasErrors)
        {
            toast.ShowToast(addMoneyToUserResult.ToString());
            return;
        }

        var updatedUserResult = await _eventOnApiService.GetUser(UserData.Instance.Username);
        if (updatedUserResult.HasErrors)
        {
            toast.ShowToast(updatedUserResult.ToString());
            return;
        }

        var hasCreatedEvents = (await _eventOnApiService.GetAllEventsFromUser(UserData.Instance.Username)).Data.Count > 0;

        UserData.Instance.SetNewUser(updatedUserResult.Data, hasCreatedEvents);

        toast.ShowToast("Money added succesfully", false);
        CloseAddMoneyPrompt();
    }

    private async Task OpenCardInfoPrompt()
    {
        var user = await _eventOnApiService.GetUser(UserData.Instance.Username);
        if (!user.HasErrors)
        {
            var hasCreatedEvents = (await _eventOnApiService.GetAllEventsFromUser(UserData.Instance.Username)).Data.Count > 0;
            UserData.Instance.SetNewUser(user.Data, hasCreatedEvents);
        }
        else
            toast.ShowToast("Can't get updated user Card info");

        _showCardInfoPromt = true;
    }

    private async Task ToggleTheme()
    {
        var newTheme = UserData.Instance.Preferences.SelectedTheme == Theme.Dark ?
            Theme.Light : Theme.Dark;

        var updatedPreferences = UserData.Instance.Preferences;
        updatedPreferences.Username = UserData.Instance.Username;
        updatedPreferences.SelectedTheme = newTheme;

        var updatePreferencesResult = await _eventOnApiService.UpdateUserPreferences(UserData.Instance.Username, updatedPreferences);
        if (updatePreferencesResult.HasErrors)
        {
            toast.ShowToast(updatePreferencesResult.ToString());
            return;
        }

        await _themeHandler.SaveThemeLocally(newTheme);
        await _themeHandler.ApplyTheme();

        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }

    private async Task SaveNotificationPreferences()
    {
        var updatedPreferences = UserData.Instance.Preferences;
        updatedPreferences.Username = UserData.Instance.Username;
        updatedPreferences.Notifications = _notificationPreferences;

        var updatePreferencesResult = await _eventOnApiService.UpdateUserPreferences(UserData.Instance.Username, updatedPreferences);
        if (updatePreferencesResult.HasErrors)
        {
            toast.ShowToast(updatePreferencesResult.ToString());
            return;
        }

        toast.ShowToast("Notifications updated", false);
        UserData.Instance.Preferences= updatedPreferences;
    }

    private void OpenDeleteAccountPrompt()
    {
        _showDeleteAccountPromt = true;
    }

    private void OpenChangeAccountPrompt()
    {
        _showChangeAccountPromt = true;
    }

    private void OpenAddMoneyPrompt()
    {
        _showAddMoneyPrompt = true;
    }

    private async Task OpenNotificationsPrompt()
    {
        var getPreferencesResult = await _eventOnApiService.GetUserPreferences(UserData.Instance.Username);
        if (getPreferencesResult.HasErrors)
        {
            toast.ShowToast(getPreferencesResult.ToString());
            return;
        }

        _notificationPreferences = getPreferencesResult.Data.Notifications;
        _showNotificationsPrompt = true;
    }

    private void CloseAddMoneyPrompt()
    {
        _showAddMoneyPrompt = false;
        _amountToAdd = 0;
    }

    private void BackToMainSettingsPage()
    {
        _showChangePasswordPromt = false;
        _showDeleteAccountPromt = false;
        _showChangeAccountPromt = false;
        _showCardInfoPromt = false;
        _showNotificationsPrompt = false;
        _notificationPreferences = new();

        newPassword = string.Empty;
        repeatedPassword = string.Empty;
    }

    private void DeleteAccount()
    {
        toast.ShowToast("Nu e implementat");
    }
}