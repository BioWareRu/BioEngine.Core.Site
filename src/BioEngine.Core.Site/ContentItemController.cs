using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.Site
{
    public abstract class ContentItemController<TContentItem, TSectionPk> : SiteController<TContentItem, TSectionPk>
        where TContentItem : ContentItem, IEntity<TSectionPk>
    {
        protected ContentItemController(SiteControllerContext<TContentItem, TSectionPk> context) : base(context)
        {
        }
    }
}