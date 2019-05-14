using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BioEngine.Core.Site
{
    public class SiteModule : WebModule<SiteModuleConfig>
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            base.ConfigureServices(services, configuration, environment);
            services.AddSingleton(Config);
            services.AddSingleton<IStartupFilter, CurrentSiteStartupFilter>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }
    }

    public class CurrentSiteStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<CurrentSiteMiddleware>();
                next(builder);
            };
        }
    }

    [UsedImplicitly]
    public class CurrentSiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SiteModuleConfig _options;

        public CurrentSiteMiddleware(RequestDelegate next, SiteModuleConfig options)
        {
            _next = next;
            _options = options;
        }

        [UsedImplicitly]
        [SuppressMessage("AsyncUsage.CSharp.Naming", "UseAsyncSuffix", Justification = "Reviewed.")]
        public async Task Invoke(HttpContext context)
        {
            var repository = context.RequestServices.GetRequiredService<SitesRepository>();
            var site = await repository.GetByIdAsync(_options.SiteId);
            if (site == null)
            {
                context.Response.StatusCode = 401;
            }
            else
            {
                context.Features.Set(new CurrentSiteFeature(site));
                await _next.Invoke(context);
            }
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
