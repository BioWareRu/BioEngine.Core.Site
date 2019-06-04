using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using cloudscribe.Web.SiteMap;
using cloudscribe.Web.SiteMap.Controllers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.Site.Controllers
{
    public abstract class BaseSiteMapController : SiteMapController
    {
        protected BaseSiteMapController(ILogger<SiteMapController> logger,
            IEnumerable<ISiteMapNodeService> nodeProviders = null) : base(logger, nodeProviders)
        {
        }

        [SuppressMessage("ReSharper", "UseAsyncSuffix")]
        public override Task<IActionResult> Index()
        {
            // https://github.com/aspnet/AspNetCore/issues/7644 may be fixed in next cloudscribe release
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            return base.Index();
        }
    }
}
