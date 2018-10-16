using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Site.Model;

namespace BioEngine.Core.Site.Filters
{
    public interface IPageFilter
    {
        bool CanProcess(Type type);

        Task<bool> ProcessPage(PageViewModelContext viewModel);

        Task<bool> ProcessEntities<TEntity, TEntityPk>(PageViewModelContext viewModel, IEnumerable<TEntity> entities)
            where TEntity : class, IEntity<TEntityPk>;
    }

    public static class PageFilterHelper
    {
        public static string GetFeatureKey<TFeature>(IEntity entity)
        {
            return $"{GetFeatureKey<TFeature>()}|{entity.GetType()}|{entity.GetId()}";
        }

        public static string GetFeatureKey<TFeature>()
        {
            return $"{typeof(TFeature)}";
        }
    }
}