using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using Rm.TwoFactorAuth.Permissions;
using Rm.TwoFactorAuth.Settings;
using Rm.TwoFactorAuth.TwoFactor;
using Rm.TwoFactorAuth.Web.Models;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using ISettingManager = Volo.Abp.SettingManagement.ISettingManager;

namespace Rm.TwoFactorAuth.Web.Controllers;


[Authorize]
[ApiController]
[Route("api/rm/two-factor")]
public class TwoFactorController : AbpController
{
    private readonly ITwoFactorAppService _twoFactorAppService;
    private readonly ISettingManager _settingManager;
     
    public TwoFactorController(ITwoFactorAppService twoFactorAppService
        , ISettingManager settingManager
        , ICurrentTenant currentTenant
       )
    {
        _twoFactorAppService = twoFactorAppService;
        _settingManager = settingManager; 
    }


    [HttpGet("setup")]
    public async Task<object> GetSetupAsync()
    {
        var s = await _twoFactorAppService.GetSetupAsync();
        return new { isTwoFactorEnabled = s.IsTwoFactorEnabled };
    }

    public record EnableInput { public string VerificationCode { get; set; } = ""; }
    public record ResetInput { public string UserId { get; set; } = ""; }

    
    [HttpPost("enable")]
    public Task EnableAsync([FromBody] EnableInput input)
        => _twoFactorAppService.EnableAsync(input.VerificationCode);

    [HttpPost("disable")]
    public Task DisableAsync()
        => _twoFactorAppService.DisableAsync();

    [HttpPost("reset")]
    public Task ResetAsync()
        => _twoFactorAppService.ResetAsync();


    [HttpPost("reset-id")]
    [Authorize(IdentityPermissions.Users.Update)]
    public Task ResetByUserIdAsync([FromBody] ResetInput input)
        => _twoFactorAppService.ResetByUserIdAsync(input.UserId);


    [HttpGet("qr")]
    public async Task<IActionResult> GetQrAsync()
    {
        var otpAuthUri = await _twoFactorAppService.GetOtpAuthUriAsync();
        using var generator = new QRCodeGenerator();
        using var qrData = generator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(qrData).GetGraphic(pixelsPerModule: 20);

        return File(png, "image/png");
    }

    [HttpGet("manual-key")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<object> GetManualKeyAsync()
    {
        var s = await _twoFactorAppService.GetSetupAsync();

        // 你現在策略：啟用後不回 secret（更安全）
        if (s.IsTwoFactorEnabled)
        {
            // 也可以改回 403
            throw new UserFriendlyException("Two-factor authentication is already enabled.");
        }

        // SharedKey 可能為 null（如果你 AppService 目前也不回）
        return new
        {
            sharedKey = s.SharedKey
        };
    }

    [HttpPost("setting")]
    [Authorize(TwoFactorAuthPermissions.SettingsManage)]
    public async Task SettingAsync([FromBody] TwoFactorAuthSettingGroupViewModel input){
        var value = input.EnforcementEnabled ? "true" : "false";
        await _settingManager.SetForCurrentTenantAsync(TwoFactorAuthSettings.Enforcement.Enabled, value);
        await _settingManager.SetForCurrentTenantAsync(TwoFactorAuthSettings.Issuer, input.Issuer);
    }

    
}
