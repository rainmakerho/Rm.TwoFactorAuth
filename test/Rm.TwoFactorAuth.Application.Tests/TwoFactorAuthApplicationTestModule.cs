using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;
using Rm.TwoFactorAuth.TwoFactor;
using System;
using System.Security.Claims;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Rm.TwoFactorAuth;

[DependsOn(
    typeof(TwoFactorAuthApplicationModule),
    typeof(TwoFactorAuthTestBaseModule)
    )]
public class TwoFactorAuthApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        // Options
        context.Services.Replace(ServiceDescriptor.Singleton<IOptions<TwoFactorAuthOptions>>(
            Options.Create(new TwoFactorAuthOptions { Issuer = "TestIssuer" })
        ));

        // UserManager mock
        var testUserId = Guid.Parse(TestUsers.AdminId); 
        var user = new IdentityUser(testUserId, TestUsers.AdminUserName, TestUsers.AdminEmail);

        var um = UserManagerMockFactory.Create();

        um.Setup(x => x.FindByIdAsync(testUserId.ToString()))
          .ReturnsAsync(user);

        um.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        um.Setup(x => x.GetTwoFactorEnabledAsync(user))
          .ReturnsAsync(false);

        um.Setup(x => x.GetAuthenticatorKeyAsync(user))
          .ReturnsAsync("USS4S5PCFPNEYUAKGSJEI45PZCQRG2Q5");

        um.Setup(x => x.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultAuthenticatorProvider,
                It.IsAny<string>()))
          .ReturnsAsync(true);

        um.Setup(x => x.SetTwoFactorEnabledAsync(user, false))
            .ReturnsAsync(IdentityResult.Success);

        um.Setup(x => x.SetTwoFactorEnabledAsync(user, true))
            .ReturnsAsync(IdentityResult.Success);

        context.Services.Replace(ServiceDescriptor.Singleton(um.Object));
        context.Services.AddSingleton(um);
    }
}
