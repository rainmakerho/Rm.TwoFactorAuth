using Microsoft.AspNetCore.Builder;
using Rm.TwoFactorAuth;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Rm.TwoFactorAuth.Web.csproj"); 
await builder.RunAbpModuleAsync<TwoFactorAuthWebTestModule>(applicationName: "Rm.TwoFactorAuth.Web");

public partial class Program
{
}
