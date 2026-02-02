using Volo.Abp.Users;
using Xunit;

namespace Rm.TwoFactorAuth.TwoFactor;

public class CurrentUser_Tests : TwoFactorAuthApplicationTestBase<TwoFactorAuthApplicationTestModule>
{

    [Fact]
    public void ICurrentUser_should_be_available_and_fake()
    {
        var currentUser = GetRequiredService<ICurrentUser>();

        Assert.True(currentUser.IsAuthenticated);
        Assert.Equal("admin", currentUser.UserName);
        Assert.Equal("admin@abp.io", currentUser.Email);
        Assert.True(currentUser.Id.HasValue);
    }
}
