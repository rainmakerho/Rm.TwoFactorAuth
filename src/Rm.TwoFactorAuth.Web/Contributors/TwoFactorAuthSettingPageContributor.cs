using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rm.TwoFactorAuth.Permissions;
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

    public async Task<bool> CheckPermissionsAsync(SettingPageCreationContext context)
    {
        var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
        return await authorizationService.IsGrantedAsync(TwoFactorAuthPermissions.SettingsManage);
    }
}
