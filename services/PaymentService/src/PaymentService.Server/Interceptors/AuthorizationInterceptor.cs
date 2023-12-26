using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.IdentityModel.Tokens;

namespace PaymentService.Server.Interceptors;

public class AuthorizationInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "Authorization")?.Value.Split(' ').LastOrDefault();

            if (token is not null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetSymmetricSecurityKey("securityKey123123123"),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var _);
            }
        }
        catch (Exception)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Token is not valid"));
        }

        return await continuation(request, context);
    }
    
    private static SymmetricSecurityKey GetSymmetricSecurityKey(string rawString)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(rawString));
    }
}