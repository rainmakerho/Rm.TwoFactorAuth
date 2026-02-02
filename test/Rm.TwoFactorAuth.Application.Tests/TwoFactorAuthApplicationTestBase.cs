using Volo.Abp.Modularity;

namespace Rm.TwoFactorAuth;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class TwoFactorAuthApplicationTestBase<TStartupModule> : TwoFactorAuthTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
