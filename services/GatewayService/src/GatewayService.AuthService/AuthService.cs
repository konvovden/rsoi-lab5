using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GatewayService.AuthService.Configurations;
using GatewayService.AuthService.Models;
using GatewayService.AuthService.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GatewayService.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserSessionKeysStorage _sessionKeysStorage;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IOptions<JwtConfiguration> jwtConfiguration,
            ILogger<AuthService> logger)
        {
            _jwtConfiguration = jwtConfiguration.Value;
            _logger = logger;
            _sessionKeysStorage = new UserSessionKeysStorage();
        }

        public string Login(Guid id, string login, List<string> permissions, string name)
        {
            var sessionKey = _sessionKeysStorage.GenerateNewSessionKey(id);

            var claimsIdentity = ClaimsCreator.CreateClaimsIdentity(new IdentityUser(id,
                login,
                permissions,
                sessionKey,
                name));

            var token = CreateJwtToken(claimsIdentity);

            return token;
        }

        public void Logout(Guid userId)
        {
            _sessionKeysStorage.RemoveUserSessionKey(userId);
        }

        public bool HttpContextContainsUserWithNeededPermissions(HttpContext httpContext, string permission)
        {
            var user = (IdentityUser?)httpContext.Items[nameof(IdentityUser)];

            if (user == null)
                return false;
            if (!user.Permissions.Contains(permission))
                return false;

            return true;
        }

        public bool UserIsLoggedIn(Guid userId)
        {
            return _sessionKeysStorage.GetUserSessionKey(userId) is not null;
        }

        public Guid? GetSessionKey(Guid userId)
        {
            return _sessionKeysStorage.GetUserSessionKey(userId);
        }

        public bool IsValidSessionKey(Guid userId, Guid sessionKey)
        {
            var userSessionKey = _sessionKeysStorage.GetUserSessionKey(userId);
            return userSessionKey == sessionKey;
        }
        
        public bool IsValidSessionKey(HttpContext httpContext)
        {
            var user = (IdentityUser?)httpContext.Items[nameof(IdentityUser)];

            if (user is null)
                return false;

            var sessionKey = _sessionKeysStorage.GetUserSessionKey(user.Id);
            
            return user.SessionKey == sessionKey;
        }

        private string CreateJwtToken(ClaimsIdentity identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (_jwtConfiguration.SecurityKey == null)
            {
                _logger.LogCritical("JwtConfiguration not loaded in {service}.", nameof(AuthService));
                
                throw new ArgumentNullException(
                    $"{nameof(_jwtConfiguration.SecurityKey)} in {nameof(_jwtConfiguration)} is null!");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(_jwtConfiguration.LifetimeHours),
                SigningCredentials = new SigningCredentials(
                    SymmetricSecurityKeysHelper.GetSymmetricSecurityKey(_jwtConfiguration.SecurityKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
