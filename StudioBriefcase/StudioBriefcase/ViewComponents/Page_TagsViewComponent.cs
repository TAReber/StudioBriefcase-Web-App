using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;

namespace StudioBriefcase.ViewComponents
{
    public class Page_TagsViewComponent : ViewComponent
    {

        PageService _pageService;

        public Page_TagsViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        //[ResponseCache(Duration = 3600)] //Not Caching Response
        public async Task<IViewComponentResult> InvokeAsync()
        {
            TagsModel tags = new TagsModel();
            tags.TagLists = await _pageService.GetLibraryTagsAsync();

            return View("~/Pages/Shared/Components/Page/_Page_Navigation_Tags.cshtml", tags);
        }

    }
}
