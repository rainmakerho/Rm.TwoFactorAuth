using Rm.TwoFactorAuth.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Rm.TwoFactorAuth.Permissions;


public class TwoFactorAuthPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            TwoFactorAuthPermissions.GroupName,
            L("Permission:RmTwoFactorAuth"));

        group.AddPermission(
            TwoFactorAuthPermissions.SettingsManage,
            L("Permission:Settings:Manage"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TwoFactorAuthResource>(name);
    }
}