using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Sitemaps;
using BioEngine.Core.Web;
using cloudscribe.Web.SiteMap;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.Site
{
    public class SiteModule : WebModule<SiteModuleConfig>
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            base.ConfigureServices(services, configuration, environment);
            services.AddSingleton(Config);
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<ISiteMapNodeService, SectionsSiteMapNodeService>();
        }
    }

    [UsedImplicitly]
    public class CurrentSiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SiteModuleConfig _options;
        private readonly ILogger<CurrentSiteMiddleware> _logger;

        public CurrentSiteMiddleware(RequestDelegate next, SiteModuleConfig options,
            ILogger<CurrentSiteMiddleware> logger)
        {
            _next = next;
            _options = options;
            _logger = logger;
        }

        [UsedImplicitly]
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Reviewed.")]
        public async Task Invoke(HttpContext context)
        {
            Entities.Site site = null;
            try
            {
                var repository = context.RequestServices.GetRequiredService<SitesRepository>();
                site = await repository.GetByIdAsync(_options.SiteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in site middleware: {errorText}", ex.ToString());
            }

            if (site == null)
            {
                throw new Exception("Site is not configured");
            }

            context.Features.Set(new CurrentSiteFeature(site));
            await _next.Invoke(context);
        }
    }

    public class SiteModuleConfig : WebModuleConfig
    {
        public SiteModuleConfig(Guid siteId)
        {
            SiteId = siteId;
        }

        public Guid SiteId { get; }
    }
}
