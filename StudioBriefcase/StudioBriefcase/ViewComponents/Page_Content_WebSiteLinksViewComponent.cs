using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class Page_Content_WebSiteLinksViewComponent : ViewComponent
    {
        PageService _pageService;
        public Page_Content_WebSiteLinksViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(PageDataMap map)
        {
            List<ID_String_Alias_Pair_Model> links = await _pageService.GetPostTypeRecord(map, "website");
            
            return View(links);
        }
    }
}
