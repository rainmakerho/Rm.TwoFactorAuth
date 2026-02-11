namespace Rm.TwoFactorAuth.Settings;

public static class TwoFactorAuthSettings
{
    public const string GroupName = "RmTwoFactorAuth";

    public const string Issuer = GroupName + ".Issuer";

    public static class Enforcement
    {
        public const string Enabled = GroupName + ".Enforcement.Enabled";
    }
}
