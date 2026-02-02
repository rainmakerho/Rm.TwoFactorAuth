using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Rm.TwoFactorAuth.Security;

[Dependency(ReplaceServices = true)]
public class FakeCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
{
    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return GetPrincipal();
    }

    private ClaimsPrincipal GetPrincipal()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(AbpClaimTypes.UserId, TestUsers.AdminId),
                    new Claim(AbpClaimTypes.UserName, TestUsers.AdminUserName),
                    new Claim(AbpClaimTypes.Email, TestUsers.AdminEmail)
                }
            )
        );
    }
}
