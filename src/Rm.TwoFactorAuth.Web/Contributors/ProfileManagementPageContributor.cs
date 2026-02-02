using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Rm.TwoFactorAuth.Localization;
using Rm.TwoFactorAuth.Web.Pages.Account.Components.ProfileManagementGroup.TwoFactorAuthentication;
using System.Threading.Tasks;
using Volo.Abp.Account.Web.ProfileManagement;
namespace Rm.TwoFactorAuth.Web.Contributors;

public class ProfileManagementPageContributor : IProfileManagementPageContributor
{
    public async Task ConfigureAsync(ProfileManagementPageCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<TwoFactorAuthResource>>();

        context.Groups.Add(
            new ProfileManagementPageGroup(
                "Volo.Abp.Account.TwoFactorAuthentication",
                l["ProfileTab:TwoFactorAuthentication"],
                typeof(AccountProfileTwoFactorAuthenticationManagementGroupViewComponent)
            )
        );
    }
}
