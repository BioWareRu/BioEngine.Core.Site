using BioEngine.Core.Entities;

namespace BioEngine.Core.Site.Model
{
    public struct BlockViewModel<T, TData> where T : ContentBlock<TData> where TData : ContentBlockData, new()
    {
        public BlockViewModel(T block, Post post)
        {
            Block = block;
            Post = post;
        }

        public T Block { get; set; }
        public Post Post { get; set; }
    }
}