using GatewayService.AuthService.Models;
using Microsoft.AspNetCore.Http;

namespace GatewayService.AuthService.Extensions;

public static class HttpContextExtensions
{
    public static IdentityUser GetIdentityUser(this HttpContext context)
    {
        var identityUser = FindIdentityUser(context);

        if (identityUser is null)
            throw new InvalidOperationException("Identity user not found");

        return identityUser;
    }
    
    public static IdentityUser? FindIdentityUser(this HttpContext context)
    {
        return (IdentityUser)context.Items[nameof(IdentityUser)];
    }
}