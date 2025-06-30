using EventOn.API.Models;
using EventOn.Components;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EventOn.Pages;

public partial class PasswordResetPage
{
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }
    [Inject]
    private IUserDataValidator _userDataValidator { get; set; }

    private Toast toast;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _passwordRepeated = string.Empty;
    private string _resetRequiredCode = string.Empty;

    private bool _codeSent;

    private async Task ResetPassword()
    {
        var passwordValidationResult = _userDataValidator.ArePasswordsValid(_password, _passwordRepeated);
        if (passwordValidationResult.HasErrors)
        {
            toast.ShowToast(passwordValidationResult.ErrorMessages);
            return;
        }

        var passwordResetRequest = new PasswordResetRequest()
        {
            Password = _password,
            Code = _resetRequiredCode
        };

        var newPasswordConfirmationResult = await _eventOnApiService.ConfirmNewPassword(_username, passwordResetRequest);
        if (newPasswordConfirmationResult.HasErrors)
        {
            toast.ShowToast(newPasswordConfirmationResult.ErrorMessages);
            return;
        }

        toast.ShowToast("Password changed succesfully", false);
        await Task.Delay(2000);
        Navigation.NavigateTo("/");
    }

    private async Task SendPasswordResetRequest()
    {
        var requestPasswordChangeResult = await _eventOnApiService.RequestPasswordChange(_username);
        if (requestPasswordChangeResult.HasErrors)
        {
            toast.ShowToast(requestPasswordChangeResult.ErrorMessages);
            return;
        }

        _codeSent = true;
    }
}