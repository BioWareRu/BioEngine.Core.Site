using BioEngine.Core.Entities;

namespace BioEngine.Core.Site
{
    public abstract class SectionController<TSection> : SiteController<TSection>
        where TSection : Section, IEntity
    {
        protected SectionController(SiteControllerContext<TSection> context) : base(context)
        {
        }
    }
}
