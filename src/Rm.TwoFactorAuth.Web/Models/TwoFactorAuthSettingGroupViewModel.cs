namespace Rm.TwoFactorAuth.Web.Models;

public class TwoFactorAuthSettingGroupViewModel
{
    public string Issuer { get; set; } = "Rm.TwoFactorAuth";
    public bool EnforcementEnabled { get; set; }
}