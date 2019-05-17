using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;
using cloudscribe.Web.SiteMap;
using Microsoft.AspNetCore.Http;

namespace BioEngine.Core.Site.Sitemaps
{
    public abstract class BaseSiteMapNodeService<T> : ISiteMapNodeService where T : class, IRoutable, IEntity
    {
        protected readonly IBioRepository<T> Repository;
        protected readonly Entities.Site Site;
        protected virtual double Priority { get; } = 0.8;
        protected virtual PageChangeFrequency Frequency { get; } = PageChangeFrequency.Weekly;

        protected BaseSiteMapNodeService(IHttpContextAccessor httpContextAccessor, IBioRepository<T> repository)
        {
            Site = httpContextAccessor.HttpContext.Features.Get<CurrentSiteFeature>().Site;
            Repository = repository;
        }

        [SuppressMessage("ReSharper", "UseAsyncSuffix")]
        public async Task<IEnumerable<ISiteMapNode>> GetSiteMapNodes(
            CancellationToken cancellationToken = new CancellationToken())
        {
            var queryContext = new QueryContext<T> {IncludeUnpublished = false};
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
                new SiteMapNode(entity.PublicUrl)
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
