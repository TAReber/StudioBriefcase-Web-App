using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class Page_Content_BasicSegment : ViewComponent
    {
        PageService _pageService;

        public Page_Content_BasicSegment(PageService pageService)
        {
            _pageService = pageService;
        }

        public Task<IViewComponentResult> InvokeAsync(Tuple<PageDataMap, uint> data)
        {
            var view = View("~/Pages/Shared/Components/Page/_Page_Content_BasicSegment.cshtml", data.Item2);
            return Task.FromResult<IViewComponentResult>(view);
        }
    }
}
