using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using cloudscribe.Web.SiteMap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BioEngine.Core.Site.Sitemaps
{
    public abstract class SectionsSiteMapNodeService : BaseSiteMapNodeService<Section>
    {
        public SectionsSiteMapNodeService(IHttpContextAccessor httpContextAccessor,
            IBioRepository<Section, ContentEntityQueryContext<Section>> repository,
            LinkGenerator linkGenerator)
            : base(httpContextAccessor, repository, linkGenerator)
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
