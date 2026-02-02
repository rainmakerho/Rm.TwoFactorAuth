

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Account.Web;
using Volo.Abp.Identity;

namespace Rm.TwoFactorAuth.Web.Pages.Account
{
    public class LoginModel : Volo.Abp.Account.Web.Pages.Account.LoginModel
    {
        public LoginModel(IAuthenticationSchemeProvider schemeProvider, IOptions<AbpAccountOptions> accountOptions, IOptions<IdentityOptions> identityOptions, IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache, IWebHostEnvironment webHostEnvironment) 
            : base(schemeProvider, accountOptions, identityOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment)
        {
        }

        protected override Task<IActionResult> TwoFactorLoginResultAsync()
        {
            TempData[TwoFactorAuthTempDataKeys.RememberMe] = LoginInput.RememberMe;
            return Task.FromResult<IActionResult>(RedirectToPage(TwoFactorAuthTempDataKeys.TwoFactorLoginUrl));
        }
    }
}
