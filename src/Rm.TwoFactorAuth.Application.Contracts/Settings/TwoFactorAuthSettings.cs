using System;
using System.Collections.Generic;
using System.Text;

namespace Rm.TwoFactorAuth.Settings;

public static class TwoFactorAuthSettings
{
    public const string GroupName = "RmTwoFactorAuth";

    public static class Enforcement
    {
        public const string Enabled = GroupName + ".Enforcement.Enabled";
    }
}
