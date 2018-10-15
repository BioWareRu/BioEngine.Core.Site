using System.Collections.Generic;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.Site.Model
{
    public struct ListModel<TEntity, TEntityPk> where TEntity : class, IEntity<TEntityPk>
    {
        public ListModel(IEnumerable<TEntity> items, int totalItems)
        {
            Items = items;
            TotalItems = totalItems;
        }

        public IEnumerable<TEntity> Items { get; }
        public int TotalItems { get; }
    }
}