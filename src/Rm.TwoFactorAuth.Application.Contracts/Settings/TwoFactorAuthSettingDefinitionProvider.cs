using Rm.TwoFactorAuth.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Rm.TwoFactorAuth.Settings;

public class TwoFactorAuthSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                TwoFactorAuthSettings.Enforcement.Enabled,
                defaultValue: "false",
                displayName: LocalizableString.Create<TwoFactorAuthResource>("Setting:TwoFactorAuth:EnforcementEnabled"),
                description: LocalizableString.Create<TwoFactorAuthResource>("Setting:TwoFactorAuth:EnforcementEnabled:Description"),
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