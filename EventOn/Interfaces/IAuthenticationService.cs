using EventOn.API.Models;
using EventOn.API.Models.Enums;
using EventOn.BusinessLogic.Helpers;

namespace EventOn.Interfaces;

public interface IAuthenticationService
{
    Task<Result> Authenticate(UserLoginRequestData userData);
    Task<Result<User>> LoadSavedCredentialsAsync();
    Task<Result> CreateNewUser(string username, string password, string email, TypeOfUser type);
    Task SaveCredentialsAsync(string username, string password);
    Task SaveCredentialsAsync(string username, string password, string token);
}