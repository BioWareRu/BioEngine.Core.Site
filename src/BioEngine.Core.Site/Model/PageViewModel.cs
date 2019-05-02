using System;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Entities.Blocks;
using BioEngine.Core.Properties;
using BioEngine.Core.Seo;
using BioEngine.Core.Site.Features;
using HtmlAgilityPack;

namespace BioEngine.Core.Site.Model
{
    public abstract class PageViewModel
    {
        public readonly Entities.Site Site;
        private readonly Section _section;
        protected readonly PropertiesProvider PropertiesProvider;


        public string SiteTitle => Site.Title;

        protected PageViewModel(PageViewModelContext context)
        {
            Site = context.Site;
            _section = context.Section;
            PropertiesProvider = context.PropertiesProvider;
            FeaturesCollection = context.PageFeaturesCollection;
        }

        protected PageFeaturesCollection FeaturesCollection { get; }

        protected Uri ImageUrl { get; set; }

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
                if (_section != null)
                {
                    seoPropertiesSet = await PropertiesProvider.GetAsync<SeoPropertiesSet>(_section);
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

        public ListViewModel(PageViewModelContext context, TEntity[] items, int totalItems, int page,
            int itemsPerPage) :
            base(context)
        {
            Items = items;
            TotalItems = totalItems;
            Page = page;
            ItemsPerPage = itemsPerPage;
        }
    }

    public class EntityViewModel<TEntity> : PageViewModel where TEntity : class, IContentEntity
    {
        public TEntity Entity { get; }

        public EntityViewModel(PageViewModelContext context, TEntity entity) : base(context)
        {
            Entity = entity;
        }

        public override async Task<PageMetaModel> GetMetaAsync()
        {
            var meta = new PageMetaModel
            {
                Title = $"{Entity.Title} / {SiteTitle}", CurrentUrl = new Uri($"{Site.Url}{Entity.PublicUrl}")
            };

            var seoPropertiesSet = await PropertiesProvider.GetAsync<SeoPropertiesSet>(Entity);
            if (seoPropertiesSet != null && !string.IsNullOrEmpty(seoPropertiesSet.Description))
            {
                meta.Description = seoPropertiesSet.Description;
            }
            else
            {
                if (Entity.Blocks.FirstOrDefault(b => b is TextBlock) is TextBlock textBlock)
                {
                    meta.Description = GetDescriptionFromHtml(textBlock.Data.Text);
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
                    meta.Keywords = string.Join(", ", taggedContentEntity.Tags.Select(t => t.Name));
                }
            }

            if (Entity.Blocks.FirstOrDefault(b => b is GalleryBlock) is GalleryBlock galleryBlock
                && galleryBlock.Data.Pictures.Length > 0)
            {
                meta.ImageUrl = galleryBlock.Data.Pictures[0].PublicUri;
            }

            return meta;
        }
    }

    public class PageViewModelContext
    {
        public PageViewModelContext(PropertiesProvider propertiesProvider,
            PageFeaturesCollection pageFeaturesCollection, Entities.Site site, Section section = null)
        {
            PropertiesProvider = propertiesProvider;
            PageFeaturesCollection = pageFeaturesCollection;
            Site = site;
            Section = section;
        }

        public PropertiesProvider PropertiesProvider { get; }
        public PageFeaturesCollection PageFeaturesCollection { get; }
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
}
