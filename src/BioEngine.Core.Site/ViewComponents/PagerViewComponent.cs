using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BioEngine.Core.Site.ViewComponents
{
    public class PagerViewComponent : ViewComponent
    {
        private readonly HttpContext _httpContext;
        private readonly IUrlHelper _urlHelper;

        public PagerViewComponent(IHttpContextAccessor accessor, IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory)
        {
            _httpContext = accessor.HttpContext;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public async Task<IViewComponentResult> InvokeAsync(int page, int itemsPerPage, int totalItems)
        {
            return await Task.Run(() =>
                View(new PagerModel(page, totalItems, itemsPerPage, ViewContext, _urlHelper, _httpContext)));
        }
    }

    public struct PagerModel
    {
        private readonly ViewContext _viewContext;
        private readonly IUrlHelper _urlHelper;
        private readonly HttpContext _httpContext;
        public int CurrentPage { get; }
        public int PageCount { get; }

        public PagerModel(int currentPage, int totalItems, int itemsPerPage, ViewContext viewContext,
            IUrlHelper urlHelper, HttpContext httpContext)
        {
            _viewContext = viewContext;
            _urlHelper = urlHelper;
            _httpContext = httpContext;
            CurrentPage = currentPage;
            PageCount = (int) Math.Ceiling((double) totalItems / itemsPerPage);
        }

        public Uri PageLink(int page)
        {
            var action = _viewContext.RouteData.Values["action"].ToString();
            var urlTemplate = WebUtility.UrlDecode(_urlHelper.Action(action, new {page = "{0}"}));
            var request = _httpContext.Request;
            urlTemplate = request.Query.Keys.Where(key => key != "page").Aggregate(urlTemplate,
                (current, key) => current + "&" + key + "=" + request.Query[key]);

            return new Uri(string.Format(urlTemplate, page.ToString()), UriKind.Relative);
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