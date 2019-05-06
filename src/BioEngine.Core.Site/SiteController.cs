using System;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site
{
    public abstract class BaseSiteController : BaseController
    {
        protected BaseSiteController(BaseControllerContext context) : base(context)
        {
        }

        protected Entities.Site Site
        {
            get
            {
                var siteFeature = HttpContext.Features.Get<CurrentSiteFeature>();
                if (siteFeature == null)
                {
                    throw new ArgumentException("CurrentSiteFeature is empty");
                }

                return siteFeature.Site;
            }
        }

        protected virtual PageViewModelContext GetPageContext()
        {
            var context = new PageViewModelContext(PropertiesProvider, Site);

            return context;
        }
    }

    public abstract class SiteController<TEntity> : BaseSiteController where TEntity : class, IEntity, IRoutable
    {
        protected SiteController(BaseControllerContext<TEntity> context) : base(context)
        {
            Repository = context.Repository;
        }

        protected int Page { get; private set; } = 1;
        protected virtual int ItemsPerPage { get; } = 20;

        [PublicAPI] protected IBioRepository<TEntity> Repository;


        [HttpGet]
        public virtual async Task<IActionResult> ListAsync()
        {
            var (items, itemsCount) = await Repository.GetAllAsync(GetQueryContext());
            return View("List",
                new ListViewModel<TEntity>(GetPageContext(), items,
                    itemsCount, Page, ItemsPerPage));
        }

        [HttpGet("page/{page}.html")]
        public virtual async Task<IActionResult> ListPageAsync(int page)
        {
            var (items, itemsCount) = await Repository.GetAllAsync(GetQueryContext(page));
            return View("List",
                new ListViewModel<TEntity>(GetPageContext(), items,
                    itemsCount, Page, ItemsPerPage));
        }

        [HttpGet("{url}.html")]
        public virtual async Task<IActionResult> ShowAsync(string url)
        {
            var entity = await Repository.GetAsync(entities => entities.Where(e => e.Url == url && e.IsPublished));
            if (entity == null)
            {
                return NotFound();
            }

            return View("Show",
                new EntityViewModel<TEntity>(GetPageContext(), entity, EntityViewMode.Entity));
        }

        [PublicAPI]
        protected QueryContext<TEntity> GetQueryContext(int page = 0)
        {
            var context = new QueryContext<TEntity> {Limit = ItemsPerPage};
            if (page > 0)
            {
                Page = page;
                context.Offset = (Page - 1) * ItemsPerPage;
            }
            else if (ControllerContext.HttpContext.Request.Query.ContainsKey("page"))
            {
                Page = int.Parse(ControllerContext.HttpContext.Request.Query["page"]);
                if (Page < 1) Page = 1;
                context.Offset = (Page - 1) * ItemsPerPage;
            }
            
            context.SetSite(Site);

            if (ControllerContext.HttpContext.Request.Query.ContainsKey("order"))
            {
                context.SetOrderByString(ControllerContext.HttpContext.Request.Query["order"]);
            }
            else
            {
                context.SetOrderByDescending(e => e.DatePublished);
            }

            return context;
        }
    }
}
