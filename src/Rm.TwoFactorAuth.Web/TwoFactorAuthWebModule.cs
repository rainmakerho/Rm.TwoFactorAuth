using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rm.TwoFactorAuth.Localization;
using Rm.TwoFactorAuth.Web.Contributors;
using Rm.TwoFactorAuth.Web.Enforcement;
using Rm.TwoFactorAuth.Web.Menus;
using Rm.TwoFactorAuth.Web.Pages.Account.Components.ProfileManagementGroup.TwoFactorAuthentication;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Account.Web.ProfileManagement;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace Rm.TwoFactorAuth.Web;

[DependsOn(
    typeof(TwoFactorAuthApplicationModule),
    typeof(TwoFactorAuthApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpMapperlyModule)
    )]
public class TwoFactorAuthWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(TwoFactorAuthResource), typeof(TwoFactorAuthWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(TwoFactorAuthWebModule).Assembly);
        });
        
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new TwoFactorAuthMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<TwoFactorAuthWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<TwoFactorAuthWebModule>();

        Configure<RazorPagesOptions>(options =>
        {
            //Configure authorization.
        });

        ConfigureProfileManagementPage(context.Configuration);
        ConfigureSettingGroupPage(context.Configuration);

    }

    private void ConfigureSettingGroupPage(IConfiguration configuration)
    {
        var enforcementOptionsKey = "RmTwoFactorAuth:Enforcement";
        Configure<EnforcementOptions>(
            configuration.GetSection(enforcementOptionsKey)
        );
        Configure<SettingManagementPageOptions>(options =>
        {
            options.Contributors.Add(new TwoFactorAuthSettingPageContributor());
        });
        
    }
    private void ConfigureProfileManagementPage(IConfiguration configuration)
    {

            //using Volo.Abp.Account.Web.ProfileManagement;
            Configure<ProfileManagementPageOptions>(options =>
            {
                //using Sun.Contributors;
                options.Contributors.AddFirst(new ProfileManagementPageContributor());
            });

        Configure<AbpBundlingOptions>(options =>
        {
            //using Volo.Abp.Account.Web.Pages.Account;
            options.ScriptBundles.Configure(
                typeof(Volo.Abp.Account.Web.Pages.Account.ManageModel).FullName,
                configuration =>
                {
                    configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/TwoFactorAuthentication/Default.js");
                });
        });

    }


}
