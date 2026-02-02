using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Rm.TwoFactorAuth.TwoFactor;

public interface ITwoFactorAppService : IApplicationService
{
    Task<GetTwoFactorSetupOutput> GetSetupAsync();
    Task<GetTwoFactorSetupOutput> ResetAsync();
    Task EnableAsync(string verificationCode);
    Task DisableAsync();
    Task<bool> VerifyAsync(string verificationCode);

    [RemoteService(false)]
    Task<string> GetOtpAuthUriAsync();  

    [RemoteService(false)]
    Task ResetByUserIdAsync(string userId);
}
