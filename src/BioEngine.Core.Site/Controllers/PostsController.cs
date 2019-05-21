using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Comments;
using BioEngine.Core.Entities;
using BioEngine.Core.Entities.Blocks;
using BioEngine.Core.Repository;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;
using WilderMinds.RssSyndication;

namespace BioEngine.Core.Site.Controllers
{
    [Route("/")]
    public class PostsController : SiteController<Post>
    {
        protected readonly TagsRepository TagsRepository;
        private readonly ICommentsProvider _commentsProvider;

        public PostsController(BaseControllerContext<Post> context, TagsRepository tagsRepository,
            ICommentsProvider commentsProvider) : base(context)
        {
            TagsRepository = tagsRepository;
            _commentsProvider = commentsProvider;
        }

        [HttpGet("posts/{url}.html")]
        [SuppressMessage("ReSharper", "Mvc.ViewNotResolved")]
        public override async Task<IActionResult> ShowAsync(string url)
        {
            var post = await Repository.GetAsync(entities => entities.Where(e => e.Url == url && e.IsPublished));
            if (post == null)
            {
                return NotFound();
            }

            var commentsData = await _commentsProvider.GetCommentsDataAsync(new IContentEntity[] {post});

            return View(new PostViewModel(GetPageContext(), post, commentsData[post.Id].count,
                commentsData[post.Id].uri, ContentEntityViewMode.Entity));
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
                return NotFound();
            }

            var context = GetQueryContext(page);
            context.SetTag(tag);

            var (items, itemsCount) = await Repository.GetAllAsync(context);
            return View("List", new ListViewModel<Post>(GetPageContext(), items,
                itemsCount, Page, ItemsPerPage) {Tag = tag});
        }

        [HttpGet("/rss.xml")]
        public async Task<IActionResult> RssAsync()
        {
            var feed = new Feed
            {
                Title = Site.Title,
                Description = "Последние публикации",
                Link = new Uri(Site.Url),
                Copyright = "(c) www.BioWare.ru"
            };

            var context = GetQueryContext();

            var posts = await Repository.GetAllAsync(context);
            var mostRecentPubDate = DateTime.MinValue;
            var commentsData =
                await _commentsProvider.GetCommentsDataAsync(posts.items.Select(p => p as IContentEntity).ToArray());
            foreach (var post in posts.items)
            {
                var postDate = post.DateAdded.DateTime;
                if (postDate > mostRecentPubDate) mostRecentPubDate = postDate;
                var postUrl = new Uri($"{Site.Url}{post.PublicUrl}");


                var item = new Item
                {
                    Title = post.Title,
                    Body = GetDescription(post),
                    Link = postUrl,
                    PublishDate = postDate,
                    Author = new Author {Name = post.Author.Name},
                };

                if (commentsData.ContainsKey(post.Id))
                {
                    item.Comments = commentsData[post.Id].uri;
                }

                foreach (var section in post.Sections)
                {
                    item.Categories.Add(section.Title);
                }

                feed.Items.Add(item);
            }


            var rss = feed.Serialize();

            return Content(rss, "text/xml; charset=utf-8");
        }

        private static string GetDescription(Post post)
        {
            var description = "";

            foreach (var block in post.Blocks)
            {
                switch (block)
                {
                    case CutBlock _:
                        return description;
                    case TextBlock textBlock:
                        description += textBlock.Data.Text;
                        break;
                    case PictureBlock pictureBlock:
                        description += $"<p style=\"text-align:center;\">{pictureBlock.Data.Picture.PublicUri}</p>";
                        break;
                    default:
                        continue;
                }
            }

            return description;
        }
    }
}
