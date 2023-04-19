using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;

namespace DigitalMarketing2.CustomHandlers
{
    public class ForbiddenResultHandler : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                // User is not authenticated
                await context.ChallengeAsync();
                return;
            }

            if (authorizeResult.Forbidden)
            {
                // User is authenticated, but does not have the required role
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access Denied");
                return;
            }

            // Allow the request to proceed
            await next(context);
        }
    }
}
