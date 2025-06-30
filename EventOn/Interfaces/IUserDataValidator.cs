using EventOn.BusinessLogic.Helpers;

namespace EventOn.Interfaces;

public interface IUserDataValidator
{
    Result ValidateUserInputs(string username, string password, string passwordRepeated, string email);
    Result ArePasswordsValid(string password, string passwordRepeated);
}