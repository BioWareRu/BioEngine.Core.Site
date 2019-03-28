using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Site.Model;

namespace BioEngine.Core.Site.Filters
{
    public interface IPageFilter
    {
        bool CanProcess(Type type);

        Task<bool> ProcessPageAsync(PageViewModelContext viewModel);

        Task<bool> ProcessEntitiesAsync<TEntity>(PageViewModelContext viewModel,
            IEnumerable<TEntity> entities)
            where TEntity : class, IEntity;
    }

    public class PageFeaturesCollection
    {
        private readonly Dictionary<string, object> _features = new Dictionary<string, object>();

        public TFeature GetFeature<TFeature>() where TFeature : class
        {
            return GetFeature<TFeature>(PageFilterHelper.GetFeatureKey<TFeature>());
        }

        public TFeature GetFeature<TFeature>(IEntity entity) where TFeature : class
        {
            return GetFeature<TFeature>(PageFilterHelper.GetFeatureKey<TFeature>(entity));
        }

        private TFeature GetFeature<TFeature>(string key) where TFeature : class
        {
            if (_features.ContainsKey(key))
            {
                return _features[key] as TFeature;
            }

            return null;
        }

        public void AddFeature<TFeature>(TFeature feature) where TFeature : class
        {
            _features.Add(PageFilterHelper.GetFeatureKey<TFeature>(), feature);
        }

        public void AddFeature<TFeature>(TFeature feature, IEntity entity) where TFeature : class
        {
            _features.Add(PageFilterHelper.GetFeatureKey<TFeature>(entity), feature);
        }
    }

    public static class PageFilterHelper
    {
        public static string GetFeatureKey<TFeature>(IEntity entity)
        {
            return $"{GetFeatureKey<TFeature>()}|{entity.GetType()}|{entity.Id}";
        }

        public static string GetFeatureKey<TFeature>()
        {
            return $"{typeof(TFeature)}";
        }
    }
}
