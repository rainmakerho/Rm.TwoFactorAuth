using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Rm.TwoFactorAuth.TwoFactor;

public class EnforcementMiddleware_Tests: TwoFactorAuthWebTestBase
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    public EnforcementMiddleware_Tests()
    {
        _userManagerMock = GetRequiredService<Mock<UserManager<IdentityUser>>>();
    }

    [Fact]
    public async Task Should_redirect_to_enroll_when_mfa_not_enabled_on_normal_page()
    {

        // Act
        var resp = await Client.GetAsync("/"); // 一般頁面

        // Assert
        resp.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        resp.Headers.Location.ShouldNotBeNull();
        resp.Headers.Location!.OriginalString.ShouldStartWith("/account/manage");
    }


    [Fact]
    public async Task Allowlisted_api_should_pass_even_when_mfa_not_enabled()
    {
        
        // Act: allowlist 內的 /api/rm/two-factor
        var resp = await Client.GetAsync("/api/rm/two-factor/setup");

        // Assert: 不應被 middleware 擋掉
        resp.StatusCode.ShouldNotBe(HttpStatusCode.Redirect);
        resp.StatusCode.ShouldNotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Non_allowlisted_api_should_return_401_when_configured()
    {
        // Arrange
        
        // Act: 非 allowlist 的 api
        var resp = await Client.GetAsync("/api/anything");

        // Assert
        resp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
}
