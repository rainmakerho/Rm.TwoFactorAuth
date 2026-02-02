using System;
using System.Collections.Generic;
using System.Text;

namespace Rm.TwoFactorAuth.TwoFactor;

public class GetTwoFactorSetupOutput
{
    public bool IsTwoFactorEnabled { get; set; }
    public string? SharedKey { get; set; }
}

