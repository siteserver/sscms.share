using Microsoft.Extensions.DependencyInjection;
using SSCMS.Share.Abstractions;
using SSCMS.Share.Core;
using SSCMS.Plugins;

namespace SSCMS.Share
{
    public class Startup : IPluginConfigureServices
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IShareManager, ShareManager>();
        }
    }
}
