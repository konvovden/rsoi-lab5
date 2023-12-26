namespace GatewayService.Server.Dto;

public class LoginCredential
{
    public string Username { get; set; }
    public string Password { get; set; }

    public LoginCredential(string username, string password)
    {
        Username = username;
        Password = password;
    }
}