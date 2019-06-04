using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Routing;
using BioEngine.Core.Web;
using cloudscribe.Web.SiteMap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BioEngine.Core.Site.Sitemaps
{
    public abstract class BaseSiteMapNodeService<T> : ISiteMapNodeService where T : class, IContentEntity
    {
        protected readonly IBioRepository<T, ContentEntityQueryContext<T>> Repository;
        protected readonly LinkGenerator LinkGenerator;
        protected readonly Entities.Site Site;
        protected virtual double Priority { get; } = 0.8;
        protected virtual PageChangeFrequency Frequency { get; } = PageChangeFrequency.Weekly;

        protected BaseSiteMapNodeService(IHttpContextAccessor httpContextAccessor,
            IBioRepository<T, ContentEntityQueryContext<T>> repository,
            LinkGenerator linkGenerator)
        {
            Site = httpContextAccessor.HttpContext.Features.Get<CurrentSiteFeature>().Site;
            Repository = repository;
            LinkGenerator = linkGenerator;
        }

        [SuppressMessage("ReSharper", "UseAsyncSuffix")]
        public async Task<IEnumerable<ISiteMapNode>> GetSiteMapNodes(
            CancellationToken cancellationToken = new CancellationToken())
        {
            var queryContext = new ContentEntityQueryContext<T> {IncludeUnpublished = false};
            queryContext.SetSite(Site);
            var entities = await Repository.GetAllAsync(queryContext);

            var result = new List<ISiteMapNode>();
            foreach (var entity in entities.items)
            {
                var nodes = await GetNodesAsync(entity);
                result.AddRange(nodes);
            }

            await AddNodesAsync(result, entities.items);
            return result;
        }

        protected virtual Task<List<ISiteMapNode>> GetNodesAsync(T entity)
        {
            var result = new List<ISiteMapNode>
            {
                new SiteMapNode(LinkGenerator.GeneratePublicUrl(entity).ToString())
                {
                    LastModified = entity.DateUpdated.DateTime, ChangeFrequency = Frequency, Priority = Priority
                }
            };
            return Task.FromResult(result);
        }

        protected virtual Task AddNodesAsync(List<ISiteMapNode> nodes, T[] entities)
        {
            return Task.CompletedTask;
        }
    }
}
