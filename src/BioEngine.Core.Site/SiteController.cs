using System;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
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
            var context = new PageViewModelContext(LinkGenerator, PropertiesProvider, Site);

            return context;
        }
    }

    public abstract class SiteController<TEntity, TRepository> : BaseSiteController
        where TEntity : class, IEntity, IRoutable, IContentEntity
        where TRepository : ContentEntityRepository<TEntity>
    {
        protected SiteController(
            BaseControllerContext<TEntity, TRepository> context)
            : base(context)
        {
            Repository = context.Repository;
        }

        protected int Page { get; private set; } = 1;
        protected virtual int ItemsPerPage { get; } = 20;

        [PublicAPI] protected IBioRepository<TEntity> Repository;


        public virtual async Task<IActionResult> ListAsync()
        {
            var (items, itemsCount) = await GetAllAsync(entities => entities.Where(e => e.IsPublished));
            return View("List", new ListViewModel<TEntity>(GetPageContext(), items,
                itemsCount, Page, ItemsPerPage));
        }

        public virtual async Task<IActionResult> ListPageAsync(int page)
        {
            var (items, itemsCount) = await GetAllAsync(entities => entities.Where(e => e.IsPublished), page);
            return View("List", new ListViewModel<TEntity>(GetPageContext(), items,
                itemsCount, Page, ItemsPerPage));
        }


        public virtual async Task<IActionResult> ShowAsync(string url)
        {
            var entity = await Repository.GetAsync(entities => entities.Where(e => e.Url == url && e.IsPublished));
            if (entity == null)
            {
                return NotFound();
            }

            return View(new EntityViewModel<TEntity>(GetPageContext(), entity, ContentEntityViewMode.Entity));
        }

        [PublicAPI]
        protected BioRepositoryQuery<T> ConfigureQuery<T>(BioRepositoryQuery<T> query, int page = 0)
            where T : class, IEntity, ISiteEntity
        {
            if (ControllerContext.HttpContext.Request.Query.ContainsKey("order"))
            {
                query = query.OrderByString(ControllerContext.HttpContext.Request.Query["order"]);
            }
            else
            {
                query = query.OrderByDescending(e => e.DateUpdated);
            }

            var offset = 0;
            if (page > 0)
            {
                Page = page;
                offset = (Page - 1) * ItemsPerPage;
            }
            else if (ControllerContext.HttpContext.Request.Query.ContainsKey("page"))
            {
                Page = int.Parse(ControllerContext.HttpContext.Request.Query["page"]);
                if (Page < 1) Page = 1;
                offset = (Page - 1) * ItemsPerPage;
            }

            return query.ForSite(Site).Skip(offset).Take(ItemsPerPage);
        }

        [PublicAPI]
        protected Task<(TEntity[] items, int itemsCount)> GetAllAsync(
            Func<BioRepositoryQuery<TEntity>, BioRepositoryQuery<TEntity>>? configureQuery, int page = 0)
        {
            return Repository.GetAllAsync(entities => ConfigureQuery(configureQuery(entities), page));
        }
    }
}
