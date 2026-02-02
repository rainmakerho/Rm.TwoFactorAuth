using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Rm.TwoFactorAuth.TwoFactor;

[Authorize]
public class TwoFactorAppService : TwoFactorAuthAppService, ITwoFactorAppService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly TwoFactorAuthOptions _options;
    private readonly ILogger<TwoFactorAppService> _logger;

    public TwoFactorAppService(UserManager<IdentityUser> userManager, ICurrentUser currentUser,
        IOptions<TwoFactorAuthOptions> twoFactorOptions
        , ILogger<TwoFactorAppService> logger)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _options = twoFactorOptions.Value;
        _logger = logger;
    }
    public async Task<GetTwoFactorSetupOutput> GetSetupAsync()
    {
        var user = await GetCurrentUserAsync();

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            return new GetTwoFactorSetupOutput
            {
                IsTwoFactorEnabled = true,
                SharedKey = null
            };
        }

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (key.IsNullOrWhiteSpace())
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var account = user.Email ?? user.UserName ?? user.Id.ToString();

        return new GetTwoFactorSetupOutput
        {
            IsTwoFactorEnabled = false,
            SharedKey = FormatKey(key!)
        };
    }

    public async Task<GetTwoFactorSetupOutput> ResetAsync()
    {
        var user = await GetCurrentUserAsync();
        await ClearTwoFactorAsync(user);
        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (key.IsNullOrWhiteSpace())
        {
            throw new AbpValidationException("Failed to reset authenticator key.");
        }
        _logger.LogInformation("CurrentUser {CurrentUserId} reset authenticator for user {userId}.", _currentUser.Id.Value,  user.Id);
        return new GetTwoFactorSetupOutput
        {
            IsTwoFactorEnabled = false,
            SharedKey = FormatKey(key!)
        };
    }

    private async Task ClearTwoFactorAsync(IdentityUser user)
    {
        // 直接強制關閉 MFA 開關，不論原本狀態為何
        var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
        result.CheckErrors();

        // 強制重置金鑰 (Reset Authenticator Key)
        await _userManager.ResetAuthenticatorKeyAsync(user);

        // 為了安全，建議同時重置 Security Stamp，強制該使用者在其他裝置重新登入
        await _userManager.UpdateSecurityStampAsync(user);
    }

    [Authorize(IdentityPermissions.Users.Update)]
    public async Task ResetByUserIdAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);

        await ClearTwoFactorAsync(user);

        _logger.LogWarning("Admin {AdminId} reset authenticator for user {userId}.", _currentUser.GetId(), user.Id);
        
    }

    public async Task EnableAsync(string verificationCode)
    {
        var user = await GetCurrentUserAsync();

        // 驗證 authenticator code
        var ok = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultAuthenticatorProvider,
            NormalizeCode(verificationCode)
        );

        if (!ok)
        {
            throw new AbpValidationException("Invalid verification code.");
        }

        var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
        result.CheckErrors();
    }

    public async Task DisableAsync()
    {
        var user = await GetCurrentUserAsync();

        var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
        result.CheckErrors();
    }

    public async Task<bool> VerifyAsync(string verificationCode)
    {
        var user = await GetCurrentUserAsync();

        return await _userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultAuthenticatorProvider,
            NormalizeCode(verificationCode)
        );
    }

    private async Task<IdentityUser> GetCurrentUserAsync()
    {
        var userId = _currentUser.GetId().ToString();
        var user = await GetUserByIdAsync(userId);

        return user;
    }

    private async Task<IdentityUser> GetUserByIdAsync(string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new AbpValidationException("The user does not exist.");
        }

        return user;
    }

    private static string NormalizeCode(string code)
        => (code ?? string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);

    // Base32 key 顯示友善（XXXXX XXXXX）
    private static string FormatKey(string unformattedKey)
    {
        var key = unformattedKey.Replace(" ", string.Empty).ToUpperInvariant();
        return string.Join(" ", Enumerable.Range(0, (key.Length + 4) / 5)
            .Select(i => key.Substring(i * 5, Math.Min(5, key.Length - i * 5))));
    }

    // otpauth://totp/{issuer}:{account}?secret=...&issuer=...&digits=6
    private static string GenerateOtpAuthUri(string issuer, string accountName, string secretKey)
    {
        static string UrlEncode(string s) => Uri.EscapeDataString(s);

        return $"otpauth://totp/{UrlEncode(issuer)}:{UrlEncode(accountName)}" +
               $"?secret={UrlEncode(secretKey)}&issuer={UrlEncode(issuer)}&digits=6";
    }

    public async Task<string> GetOtpAuthUriAsync()
    {
        var user = await GetCurrentUserAsync();

        if (user.TwoFactorEnabled)
            throw new AbpValidationException("Two-factor authentication is already enabled.");

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (key.IsNullOrWhiteSpace())
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var issuer = _options.Issuer;
        var account = user.Email ?? user.UserName ?? user.Id.ToString();

        return GenerateOtpAuthUri(issuer, account, key!);
    }

    

}
