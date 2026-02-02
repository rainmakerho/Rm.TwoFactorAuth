using Microsoft.AspNetCore.Builder;
namespace Rm.TwoFactorAuth.Web.Enforcement;

public static class EnforcementMiddlewareExtensions
{
    public static IApplicationBuilder UseEnforcementTwoFactorAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EnforcementMiddleware>();
    }
}