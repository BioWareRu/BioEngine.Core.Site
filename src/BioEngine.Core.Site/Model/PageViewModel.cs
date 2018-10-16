using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Seo;
using BioEngine.Core.Settings;
using BioEngine.Core.Site.Filters;
using HtmlAgilityPack;

namespace BioEngine.Core.Site.Model
{
    public abstract class PageViewModel
    {
        private readonly Entities.Site _site;
        private readonly Section _section;
        private readonly SettingsProvider _settingsProvider;
        private readonly Dictionary<string, object> _features;

        public TFeature GetFeature<TFeature>() where TFeature : class
        {
            return GetFeature<TFeature>(PageFilterHelper.GetFeatureKey<TFeature>());
        }

        public TFeature GetFeature<TFeature>(IEntity entity) where TFeature : class
        {
            return GetFeature<TFeature>(PageFilterHelper.GetFeatureKey<TFeature>(entity));
        }

        private TFeature GetFeature<TFeature>(string key) where TFeature : class
        {
            if (_features.ContainsKey(key))
            {
                return _features[key] as TFeature;
            }

            return null;
        }

        public string SiteTitle => _site.Title;

        protected PageViewModel(PageViewModelContext context)
        {
            _site = context.Site;
            _section = context.Section;
            _settingsProvider = context.SettingsProvider;
            _features = context.Features;
        }

        protected Uri ImageUrl { get; set; }

        private PageMetaModel _meta;

        public virtual async Task<PageMetaModel> GetMeta()
        {
            if (_meta == null)
            {
                _meta = new PageMetaModel {Title = _site.Title, CurrentUrl = new Uri(_site.Url)};
                SeoSettings seoSettings = null;
                if (_section != null)
                {
                    seoSettings = await _settingsProvider.Get<SeoSettings>(_section);
                }

                if (seoSettings == null)
                {
                    seoSettings = await _settingsProvider.Get<SeoSettings>(_site);
                }

                if (seoSettings != null)
                {
                    _meta.Description = seoSettings.Description;
                    _meta.Keywords = seoSettings.Keywords;
                }
            }

            return _meta;
        }

        public virtual Uri GetImageUrl()
        {
            return ImageUrl;
        }

        public void SetImageUrl(Uri imageUrl)
        {
            ImageUrl = imageUrl;
        }


        public static string GetDescriptionFromHtml(string html)
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

        public Uri GetImageFromHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var contentBlock = htmlDoc.DocumentNode.Descendants("img").FirstOrDefault();
            var url = contentBlock?.GetAttributeValue("src", "");
            Uri uri = null;
            if (!string.IsNullOrEmpty(url))
            {
                var parsed = Uri.TryCreate(url, UriKind.Absolute, out uri);
                if (!parsed)
                {
                    Uri.TryCreate(_site.Url + url, UriKind.Absolute, out uri);
                }
            }

            return uri;
        }
    }

    public class ListViewModel<TEntity, TEntityPk> : PageViewModel where TEntity : class, IEntity<TEntityPk>
    {
        public IEnumerable<TEntity> Items { get; }
        public int TotalItems { get; }

        public ListViewModel(PageViewModelContext context, IEnumerable<TEntity> items, int totalItems) :
            base(context)
        {
            Items = items;
            TotalItems = totalItems;
        }
    }

    public class EntityViewModel<TEntity, TEntityPk> : PageViewModel where TEntity : class, IEntity<TEntityPk>
    {
        public TEntity Entity { get; }

        public EntityViewModel(PageViewModelContext context, TEntity entity) : base(context)
        {
            Entity = entity;
        }
    }

    public class PageViewModelContext
    {
        public PageViewModelContext(SettingsProvider settingsProvider, Entities.Site site, Section section = null)
        {
            SettingsProvider = settingsProvider;
            Site = site;
            Section = section;
        }

        public SettingsProvider SettingsProvider { get; }
        public Entities.Site Site { get; }
        public Section Section { get; }
        public Dictionary<string, object> Features { get; } = new Dictionary<string, object>();

        public void AddFeature<TFeature>(TFeature feature) where TFeature : class
        {
            Features.Add(PageFilterHelper.GetFeatureKey<TFeature>(), feature);
        }

        public void AddFeature<TFeature>(TFeature feature, IEntity entity) where TFeature : class
        {
            Features.Add(PageFilterHelper.GetFeatureKey<TFeature>(entity), feature);
        }
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