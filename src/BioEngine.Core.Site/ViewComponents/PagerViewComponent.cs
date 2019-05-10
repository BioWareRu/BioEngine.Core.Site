using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BioEngine.Core.Site.ViewComponents
{
    public class PagerViewComponent : ViewComponent
    {
        private readonly IUrlHelper _urlHelper;

        public PagerViewComponent(IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public async Task<IViewComponentResult> InvokeAsync(int page, int itemsPerPage, int totalItems)
        {
            return await Task.Run(() =>
                View(new PagerModel(page, totalItems, itemsPerPage, ViewContext, _urlHelper)));
        }
    }

    public struct PagerModel
    {
        private readonly ViewContext _viewContext;
        private readonly IUrlHelper _urlHelper;
        public int CurrentPage { get; }
        public int PageCount { get; }

        public PagerModel(int currentPage, int totalItems, int itemsPerPage, ViewContext viewContext,
            IUrlHelper urlHelper)
        {
            _viewContext = viewContext;
            _urlHelper = urlHelper;
            CurrentPage = currentPage;
            PageCount = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        }

        public Uri PageLink(int page)
        {
            var action = _viewContext.RouteData.Values["action"].ToString();
            var controller = _viewContext.RouteData.Values["controller"].ToString();
            var actionParams = _viewContext.RouteData.Values.Where(v => v.Key != "action" && v.Key != "controller")
                .ToDictionary(v => v.Key, v => v.Value);
            if (page > 1)
            {
                action = action.EndsWith("Page") ? action : action + "Page";
                if (actionParams.ContainsKey("page"))
                {
                    actionParams["page"] = page;
                }
                else
                {
                    actionParams.Add("page", page);
                }
            }
            else
            {
                actionParams.Remove("page");
                action = action.Replace("Page", "");
            }

            var url = _urlHelper.Action(action, controller, actionParams);
            return new Uri(url, UriKind.Relative);
        }

        public Uri FirstLink()
        {
            return PageLink(1);
        }

        public Uri LastLink()
        {
            return PageLink(PageCount);
        }

        public Uri PrevLink()
        {
            return CurrentPage > 1 ? PageLink(CurrentPage - 1) : null;
        }

        public Uri NextLink()
        {
            return CurrentPage < PageCount ? PageLink(CurrentPage + 1) : null;
        }
    }
}
