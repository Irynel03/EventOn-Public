namespace EventOn.API.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Expires { get; set; } = string.Empty;
}