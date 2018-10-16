using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.Site
{
    public abstract class SectionController<TSection, TSectionPk> : SiteController<TSection, TSectionPk>
        where TSection : Section, IEntity<TSectionPk>
    {
        protected SectionController(SiteControllerContext<TSection, TSectionPk> context) : base(context)
        {
        }
    }
}