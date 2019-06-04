using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.Site.Controllers
{
    public abstract class BaseErrorsController : BaseSiteController
    {
        protected BaseErrorsController(BaseControllerContext context) : base(context)
        {
        }

        public virtual IActionResult Error(int errorCode, [FromServices] ILogger<BaseErrorsController> logger)
        {
            if (errorCode == StatusCodes.Status404NotFound)
            {
                var feature = HttpContext.Features.Get<IHttpRequestFeature>();
                logger.LogError($"Page not found. Url: {feature.RawTarget}");
            }

            // ReSharper disable once Mvc.ViewNotResolved
            return View(new ErrorsViewModel(GetPageContext(), errorCode));
        }
    }
}
