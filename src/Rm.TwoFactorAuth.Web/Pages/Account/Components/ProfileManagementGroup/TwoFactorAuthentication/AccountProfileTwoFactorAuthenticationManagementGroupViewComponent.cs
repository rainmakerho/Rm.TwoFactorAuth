using Microsoft.AspNetCore.Mvc;
using Rm.TwoFactorAuth.TwoFactor;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;


namespace Rm.TwoFactorAuth.Web.Pages.Account.Components.ProfileManagementGroup.TwoFactorAuthentication;

[Widget(ScriptFiles = new[] { "/Pages/Account/Components/ProfileManagementGroup/TwoFactorAuthentication/Default.js" })]
[ViewComponent(Name = "AccountProfileTwoFactorAuthenticationManagementGroup")]
public class AccountProfileTwoFactorAuthenticationManagementGroupViewComponent : AbpViewComponent
{
    private readonly ITwoFactorAppService _twoFactorAppService;

    public AccountProfileTwoFactorAuthenticationManagementGroupViewComponent(
        ITwoFactorAppService twoFactorAppService)
    {
        _twoFactorAppService = twoFactorAppService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var setup = await _twoFactorAppService.GetSetupAsync();

        return View("~/Pages/Account/Components/ProfileManagementGroup/TwoFactorAuthentication/Default.cshtml",
            new TwoFactorAuthModel
            {
                IsEnabled = setup.IsTwoFactorEnabled
            });
    }

    public class TwoFactorAuthModel
    {
        public bool IsEnabled { get; set; }
    }
}
