using System;
using System.Collections.Generic;

namespace Rm.TwoFactorAuth.Web.Enforcement;

public class EnforcementOptions
{
    [Obsolete("Deprecated since v10.0.3. Use ABP setting key 'TwoFactorAuth.Enforcement.Enabled' for per-tenant enforcement. This option will be removed in a future release.")]
    public bool Enabled { get; set; } = false;

    public List<string> AllowPathPrefixes { get; set; } = new()
    {
        "/account/login",
        "/account/loginwith2fa",
        "/account/logout",
        "/account/manage",
        "/abp",      
        "/api/abp",
        "/api/rm/two-factor",   
        "/health",
        "/css", "/js", "/lib", "/images", "/favicon", "/assets"
    };

    public string EnrollPath { get; set; } = "/Account/Manage";

    public bool ApiReturnUnauthorizedInsteadOfRedirect { get; set; } = true;
}
