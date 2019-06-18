using System;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.Entities;
using BioEngine.Core.Entities.Blocks;
using BioEngine.Core.Properties;
using BioEngine.Core.Routing;
using BioEngine.Core.Seo;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Routing;

namespace BioEngine.Core.Site.Model
{
    public abstract class PageViewModel
    {
        public readonly Entities.Site Site;
        public readonly Section Section;
        protected readonly PropertiesProvider PropertiesProvider;
        protected readonly LinkGenerator LinkGenerator;

        protected PageViewModel(PageViewModelContext context)
        {
            Site = context.Site;
            Section = context.Section;
            PropertiesProvider = context.PropertiesProvider;
            LinkGenerator = context.LinkGenerator;
        }


        private PageMetaModel _meta;

        public Task<TPropertySet> GetSitePropertiesAsync<TPropertySet>() where TPropertySet : PropertiesSet, new()
        {
            return PropertiesProvider.GetAsync<TPropertySet>(Site);
        }

        public virtual async Task<PageMetaModel> GetMetaAsync()
        {
            if (_meta == null)
            {
                _meta = new PageMetaModel {Title = Site.Title, CurrentUrl = new Uri(Site.Url)};
                SeoPropertiesSet seoPropertiesSet = null;
                if (Section != null)
                {
                    seoPropertiesSet = await PropertiesProvider.GetAsync<SeoPropertiesSet>(Section);
                }

                if (seoPropertiesSet == null)
                {
                    seoPropertiesSet = await PropertiesProvider.GetAsync<SeoPropertiesSet>(Site);
                }

                if (seoPropertiesSet != null)
                {
                    _meta.Description = seoPropertiesSet.Description;
                    _meta.Keywords = seoPropertiesSet.Keywords;
                }
            }

            return _meta;
        }

        protected string GetDescriptionFromHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            string description = null;

            foreach (var childNode in htmlDoc.DocumentNode.ChildNodes.Where(x => x.Name == "p" || x.Name == "div"))
            {
                var childText = HtmlEntity.DeEntitize(childNode.InnerText.Trim('\r', '\n')).Trim();
                if (!string.IsNullOrWhiteSpace(childText))
                {
                    description = childText;
                    break;
                }
            }

            return description;
        }
    }

    public class ListViewModel<TEntity> : PageViewModel where TEntity : class, IEntity
    {
        public TEntity[] Items { get; }
        public int TotalItems { get; }

        public int Page { get; }
        public int ItemsPerPage { get; }

        public Tag[] Tags { get; set; }

        public ListViewModel(PageViewModelContext context, TEntity[] items, int totalItems, int page,
            int itemsPerPage) :
            base(context)
        {
            Items = items;
            TotalItems = totalItems;
            Page = page;
            ItemsPerPage = itemsPerPage;
        }

        public override async Task<PageMetaModel> GetMetaAsync()
        {
            var meta = await base.GetMetaAsync();
            if (Tags != null && Tags.Any())
            {
                meta.Title = $"{string.Join(", ", Tags.Select(t => t.Title))} / {Site.Title}";
            }

            return meta;
        }

        public PageViewModelContext GetContext()
        {
            return new PageViewModelContext(LinkGenerator, PropertiesProvider, Site, Section);
        }
    }

    public class SectionContentListViewModel<TSection, TContent> : ListViewModel<TContent>
        where TContent : ContentItem where TSection : Section
    {
        public new TSection Section { get; }

        public SectionContentListViewModel(PageViewModelContext context, TSection section, TContent[] items,
            int totalItems, int page,
            int itemsPerPage) : base(context, items, totalItems, page, itemsPerPage)
        {
            Section = section;
        }
    }

    public class EntityViewModel<TEntity> : PageViewModel where TEntity : class, IEntity, IRoutable
    {
        public TEntity Entity { get; }
        public ContentEntityViewMode Mode { get; }

        public EntityViewModel(PageViewModelContext context, TEntity entity,
            ContentEntityViewMode mode = ContentEntityViewMode.List)
            : base(context)
        {
            Entity = entity;
            Mode = mode;
        }

        public override async Task<PageMetaModel> GetMetaAsync()
        {
            var path = LinkGenerator.GeneratePublicUrl(Entity);
            var meta = new PageMetaModel
            {
                Title = $"{Entity.Title} / {Site.Title}", CurrentUrl = new Uri($"{Site.Url}{path}")
            };

            var seoPropertiesSet = await PropertiesProvider.GetAsync<SeoPropertiesSet>(Entity);
            if (seoPropertiesSet != null && !string.IsNullOrEmpty(seoPropertiesSet.Description))
            {
                meta.Description = seoPropertiesSet.Description;
            }
            else
            {
                if (Entity is IContentEntity contentEntity)
                {
                    if (contentEntity.Blocks.FirstOrDefault(b => b is TextBlock) is TextBlock textBlock)
                    {
                        meta.Description = GetDescriptionFromHtml(textBlock.Data.Text);
                    }
                }
            }

            if (seoPropertiesSet != null && !string.IsNullOrEmpty(seoPropertiesSet.Keywords))
            {
                meta.Keywords = seoPropertiesSet.Keywords;
            }
            else
            {
                if (Entity is ITaggedContentEntity taggedContentEntity)
                {
                    meta.Keywords = string.Join(", ", taggedContentEntity.Tags.Select(t => t.Title));
                }
            }

            if (Entity is IContentEntity contEntity)
            {
                if (contEntity.Blocks.FirstOrDefault(b => b is GalleryBlock) is GalleryBlock galleryBlock
                    && galleryBlock.Data.Pictures.Length > 0)
                {
                    meta.ImageUrl = galleryBlock.Data.Pictures[0].PublicUri;
                }
            }

            return meta;
        }

        public EntityViewModel<IContentEntity> ContentEntityViewModel()
        {
            if (Entity is IContentEntity contentEntity)
            {
                return new EntityViewModel<IContentEntity>(
                    new PageViewModelContext(LinkGenerator, PropertiesProvider, Site, Section),
                    contentEntity,
                    Mode);
            }

            throw new ArgumentException($"Entity {Entity} is not IContentEntity");
        }
    }

    public class ContentItemViewModel : EntityViewModel<ContentItem>
    {
        public int CommentsCount { get; }
        public Uri CommentsUri { get; }

        public ContentItemViewModel(PageViewModelContext context, ContentItem entity, int commentsCount,
            Uri commentsUri,
            ContentEntityViewMode mode = ContentEntityViewMode.List) :
            base(context, entity, mode)
        {
            CommentsCount = commentsCount;
            CommentsUri = commentsUri;
        }
    }

    public class PageViewModelContext
    {
        public PageViewModelContext(LinkGenerator linkGenerator, PropertiesProvider propertiesProvider,
            Entities.Site site, Section section = null)
        {
            LinkGenerator = linkGenerator;
            PropertiesProvider = propertiesProvider;
            Site = site;
            Section = section;
        }

        public LinkGenerator LinkGenerator { get; }
        public PropertiesProvider PropertiesProvider { get; }
        public Entities.Site Site { get; }
        public Section Section { get; }
    }

    public class PageMetaModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri CurrentUrl { get; set; }
    }

    public class ErrorsViewModel : PageViewModel
    {
        public int ErrorCode { get; }

        public ErrorsViewModel(PageViewModelContext context, int errorCode) : base(context)
        {
            ErrorCode = errorCode;
        }
    }
}
