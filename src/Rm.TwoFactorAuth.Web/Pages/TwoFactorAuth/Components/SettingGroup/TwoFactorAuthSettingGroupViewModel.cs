namespace Rm.TwoFactorAuth.Web.Pages.TwoFactorAuth.Components.SettingGroup;

public class TwoFactorAuthSettingGroupViewModel
{
    public string Title { get; set; } = "Two-factor authentication";
    public string EnforcementLabel { get; set; } = "Enforce two-factor authentication (MFA)";
    public string SaveText { get; set; } = "Save";

    public bool EnforcementEnabled { get; set; }
}