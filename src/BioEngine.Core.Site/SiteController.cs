using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Site.Model;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.Site
{
    public abstract class SiteController<TEntity, TEntityPk> : BaseController where TEntity : class, IEntity<TEntityPk>
    {
        protected SiteController(BaseControllerContext<TEntity, TEntityPk> context) : base(context)
        {
            Repository = context.Repository;
        }

        protected IBioRepository<TEntity, TEntityPk> Repository;

        [HttpGet]
        public virtual async Task<IActionResult> List()
        {
            var result = await Repository.GetAll(GetQueryContext());
            return View("List", new ListModel<TEntity, TEntityPk>(result.items, result.itemsCount));
        }

        [HttpGet("{id}-{url}.html")]
        public virtual async Task<IActionResult> Show(TEntityPk id, string url)
        {
            var entity = await Repository.GetById(id);
            return View("Show", entity);
        }

        protected QueryContext<TEntity, TEntityPk> GetQueryContext()
        {
            var context = new QueryContext<TEntity, TEntityPk> {IncludeUnpublished = true};
            if (ControllerContext.HttpContext.Request.Query.ContainsKey("limit"))
            {
                context.Limit = int.Parse(ControllerContext.HttpContext.Request.Query["limit"]);
            }

            if (ControllerContext.HttpContext.Request.Query.ContainsKey("offset"))
            {
                context.Offset = int.Parse(ControllerContext.HttpContext.Request.Query["offset"]);
            }

            if (ControllerContext.HttpContext.Request.Query.ContainsKey("order"))
            {
                context.SetOrderByString(ControllerContext.HttpContext.Request.Query["order"]);
            }

            return context;
        }
    }
}