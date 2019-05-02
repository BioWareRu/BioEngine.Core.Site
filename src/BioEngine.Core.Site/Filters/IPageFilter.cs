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

    public static class PageFilterHelper
    {
        public static string GetFeatureKey<TFeature>(IEntity entity)
        {
            return $"{GetFeatureKey<TFeature>()}|{entity.GetType()}|{entity.Id.ToString()}";
        }

        public static string GetFeatureKey<TFeature>()
        {
            return $"{typeof(TFeature)}";
        }
    }
}
