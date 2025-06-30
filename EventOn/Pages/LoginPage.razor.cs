using EventOn.API.Models;
using EventOn.BusinessLogic.Services;
using EventOn.Components;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Maui.Devices.Sensors;

namespace EventOn.Pages;

public partial class LoginPage
{
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private IAuthenticationService _authenticationService { get; set; }
    [Inject]
    private IUserLocalDataService _localDataService { get; set; }

    private Toast toast;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _autoLoginInProgress = false;
    private bool _showOfflineOrdersPrompt = false;

    protected override async Task OnInitializedAsync()
    {
        await TryAutoLogin();
        _autoLoginInProgress = false;
    }

    private async Task TryAutoLogin()
    {
        _autoLoginInProgress = true;
        var loadUserDataResult = await _authenticationService.LoadSavedCredentialsAsync();
        if (loadUserDataResult.HasErrors)
        {
            toast.ShowToast(loadUserDataResult.ToString());
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                toast.ShowToast("No Internet connection");
                CheckForOfflineOrders();
            }
            return;
        }

        if (string.IsNullOrWhiteSpace(loadUserDataResult.Data.Username) || string.IsNullOrWhiteSpace(loadUserDataResult.Data.Password))
            return;

        var userData = new UserLoginRequestData()
        {
            Username = loadUserDataResult.Data.Username,
            Password = loadUserDataResult.Data.Password
        };

        if (string.IsNullOrWhiteSpace(userData.Username) || string.IsNullOrWhiteSpace(userData.Password))
            return;

        var authenticateResult = await _authenticationService.Authenticate(userData);
        if (authenticateResult.HasErrors)
        {
            toast.ShowToast(authenticateResult.ToString());
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                toast.ShowToast("No Internet connection");
                CheckForOfflineOrders();
            }
        }
        else
            Navigation.NavigateTo("/feed");
    }

    private async Task Login()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            toast.ShowToast("No Internet connection");
            CheckForOfflineOrders();
            return;
        }

        var userData = new UserLoginRequestData()
        {
            Username = _username,
            Password = _password
        };

        var authenticateResult = await _authenticationService.Authenticate(userData);
        if (authenticateResult.HasErrors)
        {
            toast.ShowToast("Invalid username or password");
            return;
        }

        toast.ShowToast("Authentification succeded", false);
        await Task.Delay(1500);
        Navigation.NavigateTo("/feed");
    }

    private void CheckForOfflineOrders()
    {
        var localOrders = _localDataService.GetOrders();
        if (localOrders != null && localOrders.Count != 0)
        {
            _showOfflineOrdersPrompt = true;
            StateHasChanged();
        }
    }

    private void NavigateToOfflineOrders()
    {
        Navigation.NavigateTo("/orders");
    }

    private void ForgotPasswordPressed()
    {
        Navigation.NavigateTo("/password-reset");
    }

    private void CreateNewAccount()
    {
        Navigation.NavigateTo("/create-account");
    }
}