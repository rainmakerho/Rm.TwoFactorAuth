using Microsoft.AspNetCore.Mvc;
using Rm.TwoFactorAuth.Settings;
using Rm.TwoFactorAuth.Web.Models;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Rm.TwoFactorAuth.Web.Pages.TwoFactorAuth.Components.SettingGroup;

public class TwoFactorAuthSettingGroupViewComponent : AbpViewComponent
{
    private readonly ISettingProvider _settingProvider;
    private readonly ISettingManager _settingManager;

    public TwoFactorAuthSettingGroupViewComponent(ISettingProvider settingProvider
        , ISettingManager settingManager)
    {
        _settingProvider = settingProvider;
        _settingManager = settingManager;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var enabledString = await _settingManager.GetOrNullForCurrentTenantAsync(TwoFactorAuthSettings.Enforcement.Enabled);  
        var enabled = bool.TryParse(enabledString, out var result) && result;
        var issuer = await _settingManager.GetOrNullForCurrentTenantAsync(TwoFactorAuthSettings.Issuer);
        var vm = new TwoFactorAuthSettingGroupViewModel
        {
            EnforcementEnabled = enabled,
            Issuer = issuer
        };

        return View("~/Pages/TwoFactorAuth/Components/SettingGroup/Default.cshtml", vm);
    }
}

