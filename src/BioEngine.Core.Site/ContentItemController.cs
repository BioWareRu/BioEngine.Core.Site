using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Web;

namespace BioEngine.Core.Site
{
    public abstract class ContentItemController<TContentItem, TSectionPk> : SiteController<TContentItem, TSectionPk>
        where TContentItem : ContentItem, IEntity<TSectionPk>
    {
        protected ContentItemController(BaseControllerContext<TContentItem, TSectionPk> context) : base(context)
        {
        }
    }
}