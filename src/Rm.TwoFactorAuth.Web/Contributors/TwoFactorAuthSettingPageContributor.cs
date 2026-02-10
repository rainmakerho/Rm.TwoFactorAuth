using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rm.TwoFactorAuth.Settings;
using Rm.TwoFactorAuth.Web.Pages.TwoFactorAuth.Components.SettingGroup;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.SettingManagement.Localization;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

namespace Rm.TwoFactorAuth.Web.Contributors;

public class TwoFactorAuthSettingPageContributor : ISettingPageContributor
{
    public Task ConfigureAsync(SettingPageCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<Localization.TwoFactorAuthResource>>();
        context.Groups.Add(
            new SettingPageGroup(
                TwoFactorAuthSettings.GroupName,
                l["TwoFactorAuthentication"],
                typeof(TwoFactorAuthSettingGroupViewComponent)
            )
        );

        return Task.CompletedTask;
    }

    public Task<bool> CheckPermissionsAsync(SettingPageCreationContext context)
    {
        // You can check the permissions here
        return Task.FromResult(true);
    }
}
