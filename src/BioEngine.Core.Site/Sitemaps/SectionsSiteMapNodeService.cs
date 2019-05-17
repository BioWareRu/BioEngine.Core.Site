using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using cloudscribe.Web.SiteMap;
using Microsoft.AspNetCore.Http;

namespace BioEngine.Core.Site.Sitemaps
{
    public class SectionsSiteMapNodeService : BaseSiteMapNodeService<Section>
    {
        public SectionsSiteMapNodeService(IHttpContextAccessor httpContextAccessor, IBioRepository<Section> repository)
            : base(httpContextAccessor, repository)
        {
        }

        protected override async Task<List<ISiteMapNode>> GetNodesAsync(Section entity)
        {
            var nodes = await base.GetNodesAsync(entity);
            nodes.Add(new SiteMapNode(entity.PostsUrl)
            {
                LastModified = entity.DateUpdated.DateTime, ChangeFrequency = Frequency, Priority = Priority
            });
            return nodes;
        }
    }
}
