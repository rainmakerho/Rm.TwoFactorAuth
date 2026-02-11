using Microsoft.AspNetCore.Mvc;
using Rm.TwoFactorAuth.Settings;
using Rm.TwoFactorAuth.Web.Models;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Settings;

namespace Rm.TwoFactorAuth.Web.Pages.TwoFactorAuth.Components.SettingGroup;

public class TwoFactorAuthSettingGroupViewComponent : AbpViewComponent
{
    private readonly ISettingProvider _settingProvider;

    public TwoFactorAuthSettingGroupViewComponent(ISettingProvider settingProvider)
    {
        _settingProvider = settingProvider;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var issuer = await _settingProvider.GetOrNullAsync(TwoFactorAuthSettings.Issuer)
                     ?? "Rm.TwoFactorAuth";
        var enabledString = await _settingProvider.GetOrNullAsync(TwoFactorAuthSettings.Enforcement.Enabled);
        var enabled = bool.TryParse(enabledString, out var result) && result;
        var vm = new TwoFactorAuthSettingGroupViewModel
        {
            EnforcementEnabled = enabled,
            Issuer = issuer
        };

        return View("~/Pages/TwoFactorAuth/Components/SettingGroup/Default.cshtml", vm);
    }
}

