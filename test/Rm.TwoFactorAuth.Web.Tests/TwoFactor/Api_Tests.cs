using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
namespace Rm.TwoFactorAuth.TwoFactor;

public class Api_Tests : TwoFactorAuthWebTestBase
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    public Api_Tests()
    {
        _userManagerMock = GetRequiredService<Mock<UserManager<IdentityUser>>>();
    }


    [Fact]
    public async Task Qr_endpoint_should_return_png()
    {
        var resp = await Client.GetAsync("/api/rm/two-factor/qr");
        resp.StatusCode.ShouldBe(HttpStatusCode.OK);
        resp.Content.Headers.ContentType!.MediaType.ShouldBe("image/png");

        var bytes = await resp.Content.ReadAsByteArrayAsync();
        bytes.Length.ShouldBeGreaterThan(100); // 確保不是空檔
    }

    private sealed class EnableRequest
    {
        [JsonPropertyName("verificationCode")]
        public string VerificationCode { get; set; } = "";
    }

    [Fact]
    public async Task Enable_should_return_400_when_code_invalid()
    {
        // Arrange
        _userManagerMock.Setup(x => x.VerifyTwoFactorTokenAsync(
                It.IsAny<IdentityUser>(),
                TokenOptions.DefaultAuthenticatorProvider,
                It.IsAny<string>()))
          .ReturnsAsync(false);

        // Act
        var resp = await Client.PostAsJsonAsync("/api/rm/two-factor/enable", new EnableRequest
        {
            VerificationCode = "000000"
        });
        var body = await resp.Content.ReadAsStringAsync();
        

        // Assert

        resp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        // 並且不應該被設成啟用
        _userManagerMock.Verify(x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), true), Times.Never);

    }

    [Fact]
    public async Task Enable_should_succeed_when_code_valid()
    {
        // Arrange: token 驗證成功
         
        // Act
        var resp = await Client.PostAsJsonAsync("/api/rm/two-factor/enable", new EnableRequest
        {
            VerificationCode = "123456"
        });

        // Assert
        resp.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        _userManagerMock.Verify(x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), true), Times.AtLeastOnce);
    }
}
