using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Validation;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Rm.TwoFactorAuth.TwoFactor;

public class TwoFactorAppService_Tests : TwoFactorAuthApplicationTestBase<TwoFactorAuthApplicationTestModule>
{
  

    private readonly ITwoFactorAppService _svc;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;

    public TwoFactorAppService_Tests()
    {
        _svc = GetRequiredService<ITwoFactorAppService>();
        _userManagerMock = GetRequiredService<Mock<UserManager<IdentityUser>>>();
    }

    [Fact]
    public async Task GetSetupAsync_Should_Return_SharedKey_When_NotEnabled()
    {
        var setup = await _svc.GetSetupAsync();

        Assert.False(setup.IsTwoFactorEnabled);
        Assert.False(string.IsNullOrWhiteSpace(setup.SharedKey));
    }

    [Fact]
    public async Task GetSetupAsync_should_not_return_shared_key_when_enabled()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(true);

        // Act
        var setup = await _svc.GetSetupAsync();

        // Assert
        Assert.True(setup.IsTwoFactorEnabled);
        Assert.True(string.IsNullOrWhiteSpace(setup.SharedKey));
    }

    [Fact]
    public async Task ResetAsync_should_reset_authenticator_key_and_disable_mfa()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(true);

        // Act
        await _svc.ResetAsync();

        // Assert: 至少應該 reset key
        _userManagerMock.Verify(
            x => x.ResetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()),
            Times.AtLeastOnce);

        // Assert: 你若在 ResetAsync 也會關閉 MFA（建議要做）
        _userManagerMock.Verify(
            x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), false),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task EnableAsync_should_throw_when_verification_code_invalid()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.VerifyTwoFactorTokenAsync(
                It.IsAny<IdentityUser>(),
                TokenOptions.DefaultAuthenticatorProvider,
                It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act + Assert
        await Assert.ThrowsAsync<AbpValidationException>(
            () => _svc.EnableAsync("000000"));

        // 並且不應該啟用
        _userManagerMock.Verify(
            x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), true),
            Times.Never);
    }

    [Fact]
    public async Task EnableAsync_should_enable_when_verification_code_valid()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.VerifyTwoFactorTokenAsync(
                It.IsAny<IdentityUser>(),
                TokenOptions.DefaultAuthenticatorProvider,
                It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _svc.EnableAsync("123456");

        // Assert
        _userManagerMock.Verify(
            x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), true),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task DisableAsync_should_disable_mfa()
    {
        // Act
        await _svc.DisableAsync();

        // Assert
        _userManagerMock.Verify(
            x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), false),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task VerifyAsync_should_return_true_or_false()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.VerifyTwoFactorTokenAsync(
                It.IsAny<IdentityUser>(),
                TokenOptions.DefaultAuthenticatorProvider,
                It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var ok = await _svc.VerifyAsync("123456");

        // Assert
        Assert.False(ok);
    }
}
