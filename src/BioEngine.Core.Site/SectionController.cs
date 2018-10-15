using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Web;

namespace BioEngine.Core.Site
{
    public abstract class SectionController<TSection, TSectionPk> : SiteController<TSection, TSectionPk>
        where TSection : Section, IEntity<TSectionPk>
    {
        protected SectionController(BaseControllerContext<TSection, TSectionPk> context) : base(context)
        {
        }
    }
}