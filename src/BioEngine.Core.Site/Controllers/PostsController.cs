using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Model;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site.Controllers
{
    [Route("/")]
    public class PostsController : SiteController<Post>
    {
        protected readonly TagsRepository TagsRepository;

        public PostsController(SiteControllerContext<Post> context, TagsRepository tagsRepository) : base(context)
        {
            TagsRepository = tagsRepository;
        }

        [HttpGet("posts/{url}.html")]
        public override Task<IActionResult> ShowAsync(string url)
        {
            return base.ShowAsync(url);
        }

        [HttpGet("posts/tag/{tagName}/page/{page}.html")]
        public virtual Task<IActionResult> ListByTagPageAsync(string tagName, int page)
        {
            return ShowListByTagAsync(tagName, page);
        }

        [HttpGet("posts/tag/{tagName}.html")]
        public virtual Task<IActionResult> ListByTagAsync(string tagName)
        {
            return ShowListByTagAsync(tagName, 0);
        }

        protected virtual async Task<IActionResult> ShowListByTagAsync(string tagName, int page)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return BadRequest();
            }

            var tag = await TagsRepository.GetAsync(q => q.Where(t => t.Name == tagName));
            if (tag == null)
            {
                return BadRequest();
            }

            var context = GetQueryContext(page);
            context.SetTag(tag);

            var (items, itemsCount) = await Repository.GetAllAsync(context);
            return View("List",
                new ListViewModel<Post>(await GetPageContextAsync(items), items,
                    itemsCount, Page, ItemsPerPage) {Tag = tag});
        }
    }
}
