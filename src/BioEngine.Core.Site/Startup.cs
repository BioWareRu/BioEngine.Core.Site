using BioEngine.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BioEngine.Core.Site
{
    public abstract class BioEngineSiteStartup : BioEngineWebStartup
    {
        protected BioEngineSiteStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override IMvcBuilder ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            return base.ConfigureMvc(mvcBuilder).AddMvcOptions(options =>
            {
                options.CacheProfiles.Add("SiteMapCacheProfile",
                    new CacheProfile {Duration = 600});
            });
        }

        protected override void ConfigureBeforeRouting(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseMiddleware<CurrentSiteMiddleware>();
        }
    }
}
