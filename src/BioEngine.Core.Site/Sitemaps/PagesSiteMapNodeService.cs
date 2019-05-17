using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using Microsoft.AspNetCore.Http;

namespace BioEngine.Core.Site.Sitemaps
{
    public class PagesSiteMapNodeService : BaseSiteMapNodeService<Page>
    {
        public PagesSiteMapNodeService(IHttpContextAccessor httpContextAccessor, IBioRepository<Page> repository) :
            base(httpContextAccessor, repository)
        {
        }
    }
}
