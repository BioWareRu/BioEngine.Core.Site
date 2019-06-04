using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.Entities;
using cloudscribe.Web.SiteMap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BioEngine.Core.Site.Sitemaps
{
    public abstract class SectionsSiteMapNodeService : BaseSiteMapNodeService<Section>
    {
        protected SectionsSiteMapNodeService(IHttpContextAccessor httpContextAccessor,
            IQueryContext<Section> queryContext,
            IBioRepository<Section> repository,
            LinkGenerator linkGenerator)
            : base(httpContextAccessor, queryContext, repository, linkGenerator)
        {
        }

        protected abstract string ContentUrlRouteName { get; }

        protected override async Task<List<ISiteMapNode>> GetNodesAsync(Section entity)
        {
            var nodes = await base.GetNodesAsync(entity);
            nodes.Add(new SiteMapNode(LinkGenerator.GetPathByName(ContentUrlRouteName, new {url = entity.Url}))
            {
                LastModified = entity.DateUpdated.DateTime, ChangeFrequency = Frequency, Priority = Priority
            });
            return nodes;
        }
    }
}
