using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Rm.TwoFactorAuth.Web.Pages.Account;

public class LoginWith2faModel : AbpPageModel
{
    private const string RememberMeTempDataKey = "remember_me";

    private readonly SignInManager<IdentityUser> _signInManager;

    public LoginWith2faModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
        Input.RememberMe =
            TempData.ContainsKey(TwoFactorAuthTempDataKeys.RememberMe) &&
            Convert.ToBoolean(TempData[RememberMeTempDataKey]);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            Alerts.Warning(L["UserLogoutOrSessionExpired"]);
            return RedirectToPage(TwoFactorAuthTempDataKeys.LoginUrl); 
        }

        var code = (Input.TwoFactorCode ?? string.Empty)
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            code,
            Input.RememberMe,
            Input.RememberMachine
        );

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        if (result.IsLockedOut)
        {
            Alerts.Warning(L["UserLockedOutMessage"]);
            return Page();
        }

        Alerts.Warning(L["VerificationCodeError"]);
        return Page();
    }

    public class InputModel
    {
        [Required]
        [StringLength(7, MinimumLength = 6)]
        [DisplayName("EnterVerificationCode")]
        public string TwoFactorCode { get; set; }

        [DisplayName("RememberDevice")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
