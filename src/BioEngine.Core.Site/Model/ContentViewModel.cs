using BioEngine.Core.Entities;

namespace BioEngine.Core.Site.Model
{
    public class ContentViewModel<T> where T : ContentItem
    {
        public ContentViewModel(T item, ContentViewMode mode)
        {
            Item = item;
            Mode = mode;
        }

        public T Item { get; }
        public ContentViewMode Mode { get; }
    }

    public enum ContentViewMode
    {
        Short,
        Full
    }
}