using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Rm.TwoFactorAuth;

[DependsOn(
    typeof(TwoFactorAuthDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class TwoFactorAuthApplicationContractsModule : AbpModule
{

}
