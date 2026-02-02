using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Rm.TwoFactorAuth.Web.Enforcement;

public class EnforcementMiddleware
{
    private readonly RequestDelegate _next;
    private readonly EnforcementOptions _options;

    public EnforcementMiddleware(RequestDelegate next, IOptions<EnforcementOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<IdentityUser> userManager)
    {
        if (!_options.Enabled || !(context.User?.Identity?.IsAuthenticated ?? false))
        {
            await _next(context);
            return;
        }

        var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant();
        var enrollPath = _options.EnrollPath.ToLowerInvariant();
        // Allowlist
        if (path.StartsWith(enrollPath) || IsAllowListed(path))
        {
            await _next(context);
            return;
        }

        

        //var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.GetUserAsync(context.User);
        if (user == null)
        {
            await _next(context);
            return;
        }

        // 強迫「一定要啟用 MFA」
        var isMfaEnabled = user.TwoFactorEnabled;
        if (!isMfaEnabled )
        {
            // API/Ajax：不要 redirect，避免前端壞掉
            if (_options.ApiReturnUnauthorizedInsteadOfRedirect && IsApiRequest(context, path))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // 帶 returnUrl，設定完 MFA 回原頁
            var returnUrl = context.Request.PathBase + context.Request.Path + context.Request.QueryString;
            var target = _options.EnrollPath;

            // 簡單處理 querystring
            var sep = target.Contains('?') ? "&" : "?";
            context.Response.Redirect(target + sep + "returnUrl=" + Uri.EscapeDataString(returnUrl));
            return;
        }

        await _next(context);
    }

    private bool IsAllowListed(string path)
        => _options.AllowPathPrefixes.Any(p => path.StartsWith((p ?? "").ToLowerInvariant()));

    private static bool IsApiRequest(HttpContext context, string path)
    {
        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            return true;

        if (context.Request.Headers.TryGetValue("Accept", out StringValues accept) &&
            accept.Any(v => v.Contains("application/json", StringComparison.OrdinalIgnoreCase)))
            return true;

        if (context.Request.Headers.TryGetValue("X-Requested-With", out var xrw) &&
            xrw.Any(v => v.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    }
}
