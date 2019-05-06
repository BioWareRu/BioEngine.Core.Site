using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site.Controllers
{
    [Route("/")]
    public class PostsController : SiteController<Post>
    {
        protected readonly TagsRepository TagsRepository;

        public PostsController(BaseControllerContext<Post> context, TagsRepository tagsRepository) : base(context)
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

        protected virtual async Task<IActionResult> ShowListByTagAsync(string tagTitle, int page)
        {
            if (string.IsNullOrEmpty(tagTitle))
            {
                return BadRequest();
            }

            var tag = await TagsRepository.GetAsync(q => q.Where(t => t.Title == tagTitle));
            if (tag == null)
            {
                return BadRequest();
            }

            var context = GetQueryContext(page);
            context.SetTag(tag);

            var (items, itemsCount) = await Repository.GetAllAsync(context);
            return View("List",
                new ListViewModel<Post>(GetPageContext(), items,
                    itemsCount, Page, ItemsPerPage) {Tag = tag});
        }
    }
}
