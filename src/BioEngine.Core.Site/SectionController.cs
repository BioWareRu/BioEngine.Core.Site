using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BioEngine.Core.Site
{
    public abstract class SectionController<TSection, TRepository> : SiteController<TSection, TRepository>
        where TSection : Section, IEntity where TRepository : ContentEntityRepository<TSection>
    {
        protected SectionController(
            BaseControllerContext<TSection, TRepository> context) :
            base(context)
        {
        }

        protected virtual async Task<IActionResult> ShowContentAsync<TContent>(string url, int page = 0)
            where TContent : ContentItem
        {
            var section = await Repository.GetAsync(entities => entities.Where(e => e.Url == url && e.IsPublished));
            if (section == null)
            {
                return NotFound();
            }

            var contentRepository = HttpContext.RequestServices.GetRequiredService<IBioRepository<TContent>>();
            var (items, itemsCount) = await contentRepository.GetAllAsync(queryable =>
                ConfigureQuery(queryable, page).ForSection(section)
                    .Where(c => c.IsPublished));
            return View("Content", new SectionContentListViewModel<TSection, TContent>(GetPageContext(section), section,
                items,
                itemsCount, Page, ItemsPerPage));
        }

        protected virtual PageViewModelContext GetPageContext(TSection section)
        {
            var context = new PageViewModelContext(LinkGenerator, PropertiesProvider, Site, section);

            return context;
        }
    }
}
