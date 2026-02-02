using Rm.TwoFactorAuth.Localization;
using Volo.Abp.Application.Services;

namespace Rm.TwoFactorAuth;

public abstract class TwoFactorAuthAppService : ApplicationService
{
    protected TwoFactorAuthAppService()
    {
        LocalizationResource = typeof(TwoFactorAuthResource);
        ObjectMapperContext = typeof(TwoFactorAuthApplicationModule);
    }
}
