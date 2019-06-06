using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Search;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site.Controllers
{
    public abstract class BaseSearchController : BaseSiteController
    {
        private readonly IEnumerable<ISearchProvider> _searchProviders;

        protected BaseSearchController(BaseControllerContext context, IEnumerable<ISearchProvider> searchProviders) :
            base(context)
        {
            _searchProviders = searchProviders;
        }

        public abstract Task<IActionResult> IndexAsync([FromQuery] string query, string block);

        protected string GetDescriptionFromHtml(string html)
        {
            var description = "";
            if (!string.IsNullOrEmpty(html))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);


                foreach (var childNode in htmlDoc.DocumentNode.ChildNodes.Where(x => x.Name == "p" || x.Name == "div"))
                {
                    var childText = HtmlEntity.DeEntitize(childNode.InnerText.Trim('\r', '\n')).Trim();
                    if (!string.IsNullOrWhiteSpace(childText))
                    {
                        description = childText;
                        break;
                    }
                }
            }

            return description;
        }

        private ISearchProvider<T> GetSearchProvider<T>() where T : BaseEntity
        {
            var provider = _searchProviders.FirstOrDefault(s => s.CanProcess(typeof(T)));
            return provider as ISearchProvider<T>;
        }

        protected async Task<IEnumerable<T>> SearchEntitiesAsync<T>(string query, int limit = 0) where T : BaseEntity
        {
            var provider = GetSearchProvider<T>();
            if (provider != null)
            {
                return await provider.SearchAsync(query, limit, Site);
            }

            return null;
        }

        protected async Task<long> CountEntitiesAsync<T>(string query) where T : BaseEntity
        {
            var provider = GetSearchProvider<T>();
            if (provider != null)
            {
                return await provider.CountAsync(query, Site);
            }

            return 0;
        }

        protected SearchBlock CreateSearchBlock<T>(string title, Uri url, long totalCount,
            IEnumerable<T> items,
            Func<T, string> getTitle, Func<T, Uri> getUrl, Func<T, string> getDesc, Func<T, DateTimeOffset> getDate)
        {
            var block = new SearchBlock(title, url, totalCount);
            foreach (var item in items)
            {
                block.AddItem(getTitle(item), getUrl(item), getDesc(item), getDate(item));
            }

            return block;
        }
    }
}
