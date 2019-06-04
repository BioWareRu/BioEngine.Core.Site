using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioEngine.Core.Site.Model
{
    public class SearchViewModel : PageViewModel
    {
        public SearchViewModel(PageViewModelContext context, string query) : base(context)
        {
            Query = query;
        }

        public string Query { get; }

        public readonly List<SearchBlock> Blocks = new List<SearchBlock>();

        public void AddBlock(SearchBlock block)
        {
            Blocks.Add(block);
        }

        public override async Task<PageMetaModel> GetMetaAsync()
        {
            var meta = await base.GetMetaAsync();
            meta.Title = $"{Query} / Поиск / {Site.Title}";
            return meta;
        }
    }
}