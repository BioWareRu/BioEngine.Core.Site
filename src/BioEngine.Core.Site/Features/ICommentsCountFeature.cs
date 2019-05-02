using System;

namespace BioEngine.Core.Site.Features
{
    public interface ICommentsCountFeature : ISiteFeature
    {
        Uri CommentsUrl { get; }
        int CommentsCount { get; }
        string CommentsCountString { get; }
    }
}
