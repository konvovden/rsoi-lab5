using System.Runtime.Serialization;

namespace GatewayService.Server.Dto;

[DataContract]
public class AuthData
{
    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }

    public AuthData(string accessToken)
    {
        AccessToken = accessToken;
    }
}