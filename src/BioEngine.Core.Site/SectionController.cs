using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.Entities;
using BioEngine.Core.Extensions;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site
{
    public abstract class SectionController<TSection, TRepository> : SiteController<TSection, TRepository>
        where TSection : Section, IEntity where TRepository : ContentEntityRepository<TSection>
    {
        private readonly ContentItemsRepository _contentItemsRepository;

        protected SectionController(
            BaseControllerContext<TSection, TRepository> context,
            ContentItemsRepository contentItemsRepository) :
            base(context)
        {
            _contentItemsRepository = contentItemsRepository;
        }

        protected virtual async Task<IActionResult> ShowContentAsync(string[] types, string url, int page)
        {
            var section = await Repository.GetAsync(entities => entities.Where(e => e.Url == url && e.IsPublished));
            if (section == null)
            {
                return NotFound();
            }

            var (items, itemsCount) = await _contentItemsRepository.GetAllAsync(queryable =>
                ConfigureQuery(queryable.ForSection(section).Where(c => types.Contains(c.Type) && c.IsPublished)));
            return View("Content", new ListViewModel<ContentItem>(GetPageContext(section), items,
                itemsCount, Page, ItemsPerPage));
        }

        public virtual Task<IActionResult> PostsAsync(string[] types, string url)
        {
            return ShowContentAsync(types, url, 0);
        }

        public virtual Task<IActionResult> PostsPageAsync(string[] types, string url, int page)
        {
            return ShowContentAsync(types, url, page);
        }

        protected virtual PageViewModelContext GetPageContext(TSection section)
        {
            var context = new PageViewModelContext(LinkGenerator, PropertiesProvider, Site, section);

            return context;
        }
    }
}
