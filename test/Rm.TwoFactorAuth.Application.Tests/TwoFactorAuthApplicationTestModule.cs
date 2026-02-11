using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq;
using Rm.TwoFactorAuth.Settings;
using Rm.TwoFactorAuth.TwoFactor;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

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
        var settingProviderMock = new Mock<ISettingProvider>();

        ConfigureSettingMocks(context);
        ConfigureUserManagerMock(context);
        // SettingManagement 依賴的 Repository mocks
        ConfigureSettingManagementRepositoryMocks(context);

    }

    private void ConfigureSettingManagementRepositoryMocks(ServiceConfigurationContext context)
    {
        // Mock ISettingDefinitionRecordRepository
        var settingDefinitionRecordRepoMock = new Mock<ISettingDefinitionRecordRepository>();
        settingDefinitionRecordRepoMock
            .Setup(x => x.GetListAsync(
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SettingDefinitionRecord>());

        context.Services.Replace(
            ServiceDescriptor.Singleton(settingDefinitionRecordRepoMock.Object));

        // Mock ISettingRepository
        var settingRepoMock = new Mock<ISettingRepository>();
        settingRepoMock
            .Setup(x => x.GetListAsync(
                It.IsAny<string[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Setting>());

        context.Services.Replace(
            ServiceDescriptor.Singleton(settingRepoMock.Object));
    }
    private void ConfigureUserManagerMock(ServiceConfigurationContext context)
    {
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
    private void ConfigureSettingMocks(ServiceConfigurationContext context)
    {
        //// Mock ISettingProvider
        //var settingProviderMock = new Mock<ISettingProvider>();

        //settingProviderMock
        //    .Setup(x => x.GetOrNullAsync(TwoFactorAuthSettings.Issuer))
        //    .ReturnsAsync("TestIssuer");

        //settingProviderMock
        //    .Setup(x => x.GetOrNullAsync(TwoFactorAuthSettings.Enforcement.Enabled))
        //    .ReturnsAsync("false");

        //context.Services.Replace(ServiceDescriptor.Singleton(settingProviderMock.Object));
        //context.Services.AddSingleton(settingProviderMock);

        // Mock ISettingManager (SettingManagement)
        // 注意：GetOrNullForCurrentTenantAsync 是 extension method，
        // 底層呼叫的是 GetOrNullAsync，所以我們 mock 這個方法
        var settingManagerMock = new Mock<ISettingManager>();

        settingManagerMock
            .Setup(x => x.GetOrNullAsync(
                TwoFactorAuthSettings.Issuer,
                It.IsAny<string>(),    // providerName
                It.IsAny<string>(),    // providerKey
                It.IsAny<bool>()))     // fallback
            .ReturnsAsync("TestIssuer");

        settingManagerMock
            .Setup(x => x.GetOrNullAsync(
                TwoFactorAuthSettings.Enforcement.Enabled,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .ReturnsAsync("false");

        context.Services.Replace(ServiceDescriptor.Singleton(settingManagerMock.Object));
        context.Services.AddSingleton(settingManagerMock);
    }
}
