using Rm.TwoFactorAuth.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Rm.TwoFactorAuth.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class TwoFactorAuthPageModel : AbpPageModel
{
    protected TwoFactorAuthPageModel()
    {
        LocalizationResourceType = typeof(TwoFactorAuthResource);
        ObjectMapperContext = typeof(TwoFactorAuthWebModule);
    }
}
