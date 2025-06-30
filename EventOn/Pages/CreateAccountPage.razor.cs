using EventOn.API.Models.Enums;
using EventOn.Components;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EventOn.Pages;

public partial class CreateAccountPage
{
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private IAuthenticationService _authenticationService { get; set; }
    [Inject]
    private IUserDataValidator _userDataValidator { get; set; }

    private Toast toast;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _passwordRepeated = string.Empty;
    private string _email = string.Empty;
    private TypeOfUser _userType = TypeOfUser.Normal;

    private async Task CreateAccount()
    {
        var validateInputsResult = _userDataValidator.ValidateUserInputs(_username, _password, _passwordRepeated, _email);
        if (validateInputsResult.HasErrors)
        {
            toast.ShowToast(validateInputsResult.ErrorMessages);
            return;
        }

        var createUserResult = await _authenticationService.CreateNewUser(_username, _password, _email, _userType);
        if (!createUserResult.HasErrors)
        {
            toast.ShowToast("Account created.", false);
            await Task.Delay(2000);
            Navigation.NavigateTo("/");
        }
        else
            toast.ShowToast(createUserResult.ToString());
    }
}