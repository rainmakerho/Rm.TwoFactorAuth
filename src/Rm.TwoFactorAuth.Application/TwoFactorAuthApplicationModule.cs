using Microsoft.Extensions.DependencyInjection;
using Rm.TwoFactorAuth.Settings;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;

namespace Rm.TwoFactorAuth;

[DependsOn(
    typeof(TwoFactorAuthApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class TwoFactorAuthApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<TwoFactorAuthApplicationModule>();
        Configure<AbpSettingOptions>(options =>
        {
            options.DefinitionProviders.Add<TwoFactorAuthSettingDefinitionProvider>();
        });
    }
}
