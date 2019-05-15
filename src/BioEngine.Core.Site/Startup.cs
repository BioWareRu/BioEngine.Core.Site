using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BioEngine.Core.Site
{
    public abstract class BioEngineStartup
    {
        protected readonly IConfiguration Configuration;

        protected BioEngineStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        public virtual void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var supportedCultures = new[] {new CultureInfo("ru-RU"), new CultureInfo("ru")};

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();

            if (env.IsProduction())
            {
                var options = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                };
                options.KnownProxies.Clear();
                options.KnownNetworks.Clear();
                options.RequireHeaderSymmetry = false;
                app.UseForwardedHeaders(options);
            }
            
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseMiddleware<CurrentSiteMiddleware>();
            
            app.UseRouting();

            ConfigureApp(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual void ConfigureApp(IApplicationBuilder app, IHostEnvironment env)
        {
        }
    }
}
