using System.Runtime.Serialization;

namespace GatewayService.Server.Dto;

[DataContract]
public class LoginCredential
{
    [DataMember(Name = "grant_type")]
    public string GrantType { get; set; }
    
    [DataMember(Name = "username")]
    public string Username { get; set; }
    
    [DataMember(Name = "password")]
    public string Password { get; set; }
    
    [DataMember(Name = "client_id")]
    public string ClientId { get; set; }
    
    [DataMember(Name = "client_secret")]
    public string ClientSecret { get; set; }

    public LoginCredential(string grantType, 
        string username,
        string password,
        string clientId,
        string clientSecret)
    {
        GrantType = grantType;
        Username = username;
        Password = password;
        ClientId = clientId;
        ClientSecret = clientSecret;
    }
}