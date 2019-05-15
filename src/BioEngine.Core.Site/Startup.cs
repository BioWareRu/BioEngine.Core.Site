using BioEngine.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BioEngine.Core.Site
{
    public abstract class BioEngineSiteStartup : BioEngineWebStartup
    {
        protected BioEngineSiteStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureBeforeRouting(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseMiddleware<CurrentSiteMiddleware>();
        }
    }
}
