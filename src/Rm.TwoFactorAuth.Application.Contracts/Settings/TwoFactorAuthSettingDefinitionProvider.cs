using Rm.TwoFactorAuth.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Rm.TwoFactorAuth.Settings;

public class TwoFactorAuthSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                TwoFactorAuthSettings.Issuer,
                defaultValue: "Rm.TwoFactorAuth",
                displayName: LocalizableString.Create<TwoFactorAuthResource>("Setting:TwoFactorAuth:Issuer"),
                description: LocalizableString.Create<TwoFactorAuthResource>("Setting:TwoFactorAuth:Issuer:Description"),
                isVisibleToClients: false
            )
            .WithProviders(
                DefaultValueSettingValueProvider.ProviderName,
                ConfigurationSettingValueProvider.ProviderName,
                TenantSettingValueProvider.ProviderName
            )
        );

        context.Add(
            new SettingDefinition(
                TwoFactorAuthSettings.Enforcement.Enabled,
                defaultValue: "false",
                displayName: LocalizableString.Create<TwoFactorAuthResource>("Setting:TwoFactorAuth:Enforcement:Enabled"),
                description: LocalizableString.Create<TwoFactorAuthResource>("Setting:TwoFactorAuth:Enforcement:Enabled:Description"),
                isVisibleToClients: true
            )
            .WithProviders(
                DefaultValueSettingValueProvider.ProviderName,
                ConfigurationSettingValueProvider.ProviderName,
                TenantSettingValueProvider.ProviderName
            )
        );

    }
}