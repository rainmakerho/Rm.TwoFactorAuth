using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Volo.Abp.Security.Claims;

namespace Rm.TwoFactorAuth;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, TestUsers.AdminId),
            new Claim(AbpClaimTypes.UserName, TestUsers.AdminUserName),
            new Claim(AbpClaimTypes.Email, TestUsers.AdminEmail),
        };

        var identity = new ClaimsIdentity(claims, TestUsers.AuthenticationType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestUsers.AuthenticationType);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
