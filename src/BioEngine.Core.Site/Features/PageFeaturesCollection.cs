using System.Collections.Generic;
using BioEngine.Core.Entities;
using BioEngine.Core.Site.Filters;

namespace BioEngine.Core.Site.Features
{
    public class PageFeaturesCollection
    {
        private readonly Dictionary<string, ISiteFeature> _features = new Dictionary<string, ISiteFeature>();

        public TFeature GetFeature<TFeature>() where TFeature : class, ISiteFeature
        {
            return GetFeature<TFeature>(PageFilterHelper.GetFeatureKey<TFeature>());
        }

        public TFeature GetFeature<TFeature>(IEntity entity) where TFeature : class, ISiteFeature
        {
            return GetFeature<TFeature>(PageFilterHelper.GetFeatureKey<TFeature>(entity));
        }

        private TFeature GetFeature<TFeature>(string key) where TFeature : class, ISiteFeature
        {
            if (_features.ContainsKey(key))
            {
                return _features[key] as TFeature;
            }

            return null;
        }

        public void AddFeature<TFeature>(TFeature feature) where TFeature : class, ISiteFeature
        {
            _features.Add(PageFilterHelper.GetFeatureKey<TFeature>(), feature);
        }

        public void AddFeature<TFeature>(TFeature feature, IEntity entity) where TFeature : class, ISiteFeature
        {
            _features.Add(PageFilterHelper.GetFeatureKey<TFeature>(entity), feature);
        }
    }
}
