using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site
{
    public abstract class SectionController<TSection> : SiteController<TSection>
        where TSection : Section, IEntity
    {
        private readonly PostsRepository _postsRepository;

        protected SectionController(BaseControllerContext<TSection> context, PostsRepository postsRepository) :
            base(context)
        {
            _postsRepository = postsRepository;
        }

        [HttpGet("{url}/about.html")]
        public Task<IActionResult> AboutAsync(string url)
        {
            return base.ShowAsync(url);
        }

        [HttpGet("{url}/posts.html")]
        protected virtual async Task<IActionResult> ShowPostsAsync(string url, int page)
        {
            var section = await Repository.GetAsync(entities => entities.Where(e => e.Url == url && e.IsPublished));
            if (section == null)
            {
                return NotFound();
            }

            var postsContext = new QueryContext<Post> {Limit = ItemsPerPage};
            BuildQueryContext(postsContext, page);
            postsContext.SetSection(section);

            var (items, itemsCount) = await _postsRepository.GetAllAsync(postsContext);
            return View("Posts", new ListViewModel<Post>(GetPageContext(section), items,
                itemsCount, Page, ItemsPerPage));
        }

        [HttpGet("{url}/posts.html")]
        public virtual Task<IActionResult> PostsAsync(string url)
        {
            return ShowPostsAsync(url, 0);
        }

        [HttpGet("{url}/posts/page/{page}.html")]
        public virtual Task<IActionResult> PostsPageAsync(string url, int page)
        {
            return ShowPostsAsync(url, page);
        }

        protected virtual PageViewModelContext GetPageContext(TSection section)
        {
            var context = new PageViewModelContext(PropertiesProvider, Site, section);

            return context;
        }
    }
}
