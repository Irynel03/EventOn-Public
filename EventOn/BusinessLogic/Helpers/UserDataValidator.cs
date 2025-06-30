using EventOn.Interfaces;
using System.Text.RegularExpressions;

namespace EventOn.BusinessLogic.Helpers;

public class UserDataValidator : IUserDataValidator
{
    public Result ValidateUserInputs(string username, string password, string passwordRepeated, string email)
    {
        var result = new Result();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordRepeated) || string.IsNullOrEmpty(email))
            result.AddError("Values can't be left empty");

        var passwordErrors = ArePasswordsValid(password, passwordRepeated);
        result.Concat(passwordErrors);

        if (!IsValidEmail(email))
            result.AddError("email is invalid");

        return result;
    }

    public Result ArePasswordsValid(string password, string reEnteredPassword)
    {
        var result = new Result();

        if (password.Length < 6)
            result.AddError("Password length is too short");

        if (reEnteredPassword.Length < 6)
            result.AddError("ReEntered Password length is too short");

        if (password != reEnteredPassword)
            result.AddError("Passwords do not match.");

        return result;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }
}