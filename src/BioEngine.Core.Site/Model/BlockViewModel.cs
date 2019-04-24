using BioEngine.Core.Entities;

namespace BioEngine.Core.Site.Model
{
    public struct BlockViewModel<T, TData> where T : ContentBlock<TData> where TData : ContentBlockData, new()
    {
        public BlockViewModel(T block, IContentEntity contentEntity)
        {
            Block = block;
            ContentEntity = contentEntity;
        }

        public T Block { get; set; }
        public IContentEntity ContentEntity { get; set; }
    }
}
