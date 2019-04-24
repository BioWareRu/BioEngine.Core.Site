using BioEngine.Core.Entities;

namespace BioEngine.Core.Site.Model
{
    public class ContentViewModel<T> where T : IContentEntity
    {
        public ContentViewModel(T item, ContentViewMode mode)
        {
            Item = item;
            Mode = mode;
        }

        public T Item { get; }
        public ContentViewMode Mode { get; }
    }

    public class ContentEntityViewModel
    {
        public ContentEntityViewModel(IContentEntity item, ContentViewMode mode)
        {
            Item = item;
            Mode = mode;
        }

        public IContentEntity Item { get; }
        public ContentViewMode Mode { get; }
    }

    public enum ContentViewMode
    {
        Short,
        Full
    }
}
