using GatewayService.AuthService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace GatewayService.AuthService.Attributes
{
    /// <summary>
    /// Атрибут для проверки прав пользователя при вызове метода.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int _unauthorizedStatusCode;
        private readonly string _permission;
        private IAuthService? _authService;

        public AuthorizeAttribute(string permission, int unauthorizedStatusCode = 401)
        {
            _permission = permission;
            _unauthorizedStatusCode = unauthorizedStatusCode;
        }

        public AuthorizeAttribute(int unauthorizedStatusCode = 401)
        {
            _unauthorizedStatusCode = unauthorizedStatusCode;
            _permission = string.Empty;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            _authService =
                (filterContext.HttpContext.RequestServices.GetService<IAuthService>())!;

            var httpContext = filterContext.HttpContext;
            var user = (IdentityUser?)httpContext.Items[nameof(IdentityUser)];

            if (user == null)
            {
                filterContext.Result = new JsonResult(new { message = "Unauthorized" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else if (AuthorizationConflictOccurred(user.Id, user.SessionKey))
            {
                filterContext.Result = new JsonResult(new
                    { message = "В ваш аккаунт был выполнен вход с другого устройства." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else if (!HasRequiredPermissions(httpContext))
            {
                filterContext.Result = new JsonResult(new { message = "Пользователь не авторизован" })
                {
                    StatusCode = _unauthorizedStatusCode
                };
            }
        }

        private bool AuthorizationConflictOccurred(Guid userId, Guid sessionKey)
        {
            return false;
        }

        private bool HasRequiredPermissions(HttpContext context)
        {
            return string.IsNullOrEmpty(_permission) ||
                   _authService!.HttpContextContainsUserWithNeededPermissions(context, _permission);
        }
    }
}