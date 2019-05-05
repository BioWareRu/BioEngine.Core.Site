using BioEngine.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site.Controllers
{
    [Route("/pages")]
    public class PagesController : SiteController<Page>
    {
        public PagesController(SiteControllerContext<Page> context) : base(context)
        {
        }
    }
}
