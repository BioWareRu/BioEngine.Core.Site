using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using cloudscribe.Web.SiteMap;
using Microsoft.AspNetCore.Http;

namespace BioEngine.Core.Site.Sitemaps
{
    public class PostsSiteMapNodeService : BaseSiteMapNodeService<Post>
    {
        protected override double Priority { get; } = 0.9;

        public PostsSiteMapNodeService(IHttpContextAccessor httpContextAccessor, IBioRepository<Post> repository) :
            base(httpContextAccessor, repository)
        {
        }

        protected override async Task AddNodesAsync(List<ISiteMapNode> nodes, Post[] entities)
        {
            await base.AddNodesAsync(nodes, entities);
            nodes.Add(new SiteMapNode("/")
            {
                Priority = 1,
                ChangeFrequency = PageChangeFrequency.Daily,
                LastModified = entities.Length > 0
                    ? entities.OrderByDescending(e => e.DateUpdated).First().DateUpdated.DateTime
                    : DateTime.Now
            });
        }
    }
}
