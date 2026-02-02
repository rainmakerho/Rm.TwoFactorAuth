using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Rm.TwoFactorAuth.Web.Menus;

public class TwoFactorAuthMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(TwoFactorAuthMenus.Prefix, displayName: "TwoFactorAuth", "~/TwoFactorAuth", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
