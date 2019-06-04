using System;
using System.Collections.Generic;

namespace BioEngine.Core.Site.Model
{
    public class SearchBlock
    {
        public string Title { get; }

        public readonly List<SearchBlockItem> Items = new List<SearchBlockItem>();

        public int Count { get; private set; }
        public long TotalCount { get; }

        public Uri Url { get; }

        public SearchBlock(string title, Uri url, long totalCount)
        {
            Title = title;
            Url = url;
            TotalCount = totalCount;
        }

        public void AddItem(string title, Uri url, string text, DateTimeOffset date)
        {
            Items.Add(new SearchBlockItem(title, url, text, date));
            Count++;
        }
    }
}