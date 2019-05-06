using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Entities.Blocks;
using BioEngine.Core.Repository;
using cloudscribe.Syndication.Models.Rss;
using Microsoft.AspNetCore.Http;

namespace BioEngine.Core.Site.Rss
{
    public class RssProvider : IChannelProvider
    {
        private readonly PostsRepository _postsRepository;
        private readonly HttpContext _httpContext;

        public RssProvider(PostsRepository postsRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _postsRepository = postsRepository;
            _httpContext = httpContextAccessor.HttpContext;
        }

        private Entities.Site Site
        {
            get
            {
                var siteFeature = _httpContext.Features.Get<CurrentSiteFeature>();
                if (siteFeature == null)
                {
                    throw new ArgumentException("CurrentSiteFeature is empty");
                }

                return siteFeature.Site;
            }
        }

        [SuppressMessage("ReSharper", "UseAsyncSuffix")]
        public async Task<RssChannel> GetChannel(CancellationToken cancellationToken = new CancellationToken())
        {
            var channel = new RssChannel
            {
                Title = Site.Title,
                Description = "Последние публикации",
                Link = new Uri(Site.Url),
                Language = CultureInfo.CurrentCulture,
                TimeToLive = 60,
                LastBuildDate = DateTime.Now
            };

            var context = new QueryContext<Post> {Limit = 20, Offset = 0};
            context.SetSite(Site);
            context.SetOrderByDescending(e => e.DateAdded);
            var posts = await _postsRepository.GetAllAsync(context);
            var mostRecentPubDate = DateTime.MinValue;
            var items = new List<RssItem>();
            foreach (var post in posts.items)
            {
                var postDate = post.DateAdded.DateTime;
                if (postDate > mostRecentPubDate) mostRecentPubDate = postDate;
                var postUrl = new Uri($"{Site.Url}{post.PublicUrl}");


                var item = new RssItem
                {
                    Title = post.Title,
                    Description = GetDescription(post),
                    Link = postUrl,
                    PublicationDate = postDate,
                    Author = post.Author.Name,
                    Guid = new RssGuid(postUrl.ToString(), true)
                };

                items.Add(item);
            }

            channel.Items = items;
            channel.PublicationDate = mostRecentPubDate;
            return channel;
        }

        public string GetDescription(Post post)
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

        public string Name { get; } = "BioWare RSS";
    }
}
