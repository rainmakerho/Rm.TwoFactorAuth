using Microsoft.AspNetCore.Mvc;
using Rm.TwoFactorAuth.Settings;
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
        var settingName = TwoFactorAuthSettings.Enforcement.Enabled;
        var enabledString = await _settingManager.GetOrNullForCurrentTenantAsync(settingName, false);  
        var enabled = bool.TryParse(enabledString, out var result) && result;
        var vm = new TwoFactorAuthSettingGroupViewModel
        {
            EnforcementEnabled = enabled
        };

        return View("~/Pages/TwoFactorAuth/Components/SettingGroup/Default.cshtml", vm);
    }
}

