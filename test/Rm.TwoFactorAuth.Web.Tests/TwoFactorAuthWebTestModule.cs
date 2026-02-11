using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Rm.TwoFactorAuth.Settings;
using Rm.TwoFactorAuth.Web;
using Rm.TwoFactorAuth.Web.Enforcement;
using Rm.TwoFactorAuth.Web.Menus;
using System;
using System.Collections.Generic;
using System.Globalization;
using Volo.Abp;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.UI.Navigation;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
namespace Rm.TwoFactorAuth;

[DependsOn(
    typeof(AbpAspNetCoreTestBaseModule),
    typeof(TwoFactorAuthWebModule),
    typeof(TwoFactorAuthApplicationTestModule)
)]
public class TwoFactorAuthWebTestModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json", false);
        builder.AddJsonFile("appsettings.secrets.json", true);
        context.Services.ReplaceConfiguration(builder.Build());

        context.Services.PreConfigure<IMvcBuilder>(builder =>
        {
            builder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(typeof(TwoFactorAuthWebModule).Assembly));
        });

        context.Services.GetPreConfigureActions<OpenIddictServerBuilder>().Clear();
        PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
        {
            options.AddDevelopmentEncryptionAndSigningCertificate = true;
        });
    }
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAuthentication(TestUsers.AuthenticationType)
        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestUsers.AuthenticationType, options => { });

        ConfigureLocalizationServices(context.Services);
        ConfigureNavigationServices(context.Services);
        ConfigureSettingMocks(context);
    }

    private void ConfigureSettingMocks(ServiceConfigurationContext context)
    {

        //// Mock ISettingProvider
        var settingProviderMock = new Mock<ISettingProvider>();

        settingProviderMock
            .Setup(x => x.GetOrNullAsync(TwoFactorAuthSettings.Issuer))
            .ReturnsAsync("TestIssuer");

        settingProviderMock
            .Setup(x => x.GetOrNullAsync(TwoFactorAuthSettings.Enforcement.Enabled))
            .ReturnsAsync("true");

        context.Services.Replace(ServiceDescriptor.Singleton(settingProviderMock.Object));
        context.Services.AddSingleton(settingProviderMock);
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
            .ReturnsAsync("true");

        context.Services.Replace(ServiceDescriptor.Singleton(settingManagerMock.Object));
        context.Services.AddSingleton(settingManagerMock);
    }

    private static void ConfigureLocalizationServices(IServiceCollection services)
    {
        var cultures = new List<CultureInfo> { new CultureInfo("en"), new CultureInfo("tr") };
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
        });
    }

    private static void ConfigureNavigationServices(IServiceCollection services)
    {
        services.Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new TwoFactorAuthMenuContributor());
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // Check for TwoFactor Auth
        app.UseEnforcementTwoFactorAuth();

        app.UseConfiguredEndpoints();
    }
}
