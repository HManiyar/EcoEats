using coremvctest.IService;
using coremvctest.Utility.Content;

namespace coremvctest.Utility.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext httpContext, IAuthenticationService _authenticationService)
        {
            var token = httpContext.Request.Cookies["AuthToken"];
            //var token = authHeader?.StartsWith("Bearer ") == true ? authHeader.Substring("Bearer ".Length).Trim() : null;
            if (token == null || token == "" || token.Equals(UserMessages.invalidCredentials))
            {
                if (IsEnabledUnauthorizedRoute(httpContext))
                    return _next(httpContext);
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return httpContext.Response.WriteAsJsonAsync(UserMessages.userNotAuthenticated);
            }
            else
            {
                if (_authenticationService.validateJwtToken(token, httpContext))
                {
                    return _next(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return httpContext.Response.WriteAsJsonAsync(UserMessages.userSessionExpired);
                }
            }
        }
        /// <summary>
        /// Determines whether the requested route is authorized without authentication.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the current request.</param>
        /// <returns>Returns true if the route is enabled for unauthorized access; otherwise, false.</returns>
        private bool IsEnabledUnauthorizedRoute(HttpContext httpContext)
        {
            List<string> enabledRoutes = new List<string>
            {
                "/FoodInventory/FoodInventorySignUp",
                "/FoodInventory/FoodInventoryLogin",
                "/Location/GetLocations",
                "/NGO/NGOSignUp",
                "/NGO/NGOLogin",
                "/",
            };
            bool isEnabledUnauthorizedRoute = false;
            if (httpContext.Request.Path.Value is not null)
            {
                isEnabledUnauthorizedRoute = enabledRoutes.Contains(httpContext.Request.Path.Value);
            }
            return isEnabledUnauthorizedRoute;
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
