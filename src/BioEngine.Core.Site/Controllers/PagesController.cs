using BioEngine.Core.Entities;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site.Controllers
{
    [Route("/pages")]
    public class PagesController : SiteController<Page>
    {
        public PagesController(BaseControllerContext<Page> context) : base(context)
        {
        }
    }
}
