using System.Collections.Generic;

namespace Rm.TwoFactorAuth.Web.Enforcement;

public class EnforcementOptions
{
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

        "/css", "/js", "/lib", "/images", "/favicon", "/assets"
    };

    public string EnrollPath { get; set; } = "/Account/Manage";

    public bool ApiReturnUnauthorizedInsteadOfRedirect { get; set; } = true;
}
