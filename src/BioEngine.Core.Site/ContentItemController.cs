using BioEngine.Core.Entities;

namespace BioEngine.Core.Site
{
    public abstract class ContentItemController<TContentItem> : SiteController<TContentItem>
        where TContentItem : Post, IEntity
    {
        protected ContentItemController(SiteControllerContext<TContentItem> context) : base(context)
        {
        }
    }
}
