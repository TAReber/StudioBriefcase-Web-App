using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class Page_MiniFrameViewComponent : ViewComponent
    {
        PageService _pageService;

        public Page_MiniFrameViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            MiniViewDataModel model = new MiniViewDataModel();
            model.tags = await _pageService.GetLibraryTagsAsync();
            model.languages = await _pageService.GetLanguages();

            return View("~/Pages/Shared/Components/Page/_Page_MiniFrame.cshtml", model);
        }

    }
}
