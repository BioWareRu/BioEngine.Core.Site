using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.Site
{
    public abstract class SectionController<TSection> : SiteController<TSection>
        where TSection : Section, IEntity
    {
        protected SectionController(BaseControllerContext<TSection> context) : base(context)
        {
        }
    }
}
