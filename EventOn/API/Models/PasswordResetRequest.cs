namespace EventOn.API.Models;

public class PasswordResetRequest
{
    public string Password { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}