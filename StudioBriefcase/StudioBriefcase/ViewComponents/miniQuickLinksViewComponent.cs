using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class miniQuickLinksViewComponent : ViewComponent
    {
        PageService _pageService;
        public miniQuickLinksViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string libraryName)
        {
            
            var modelList = await _pageService.GetLibraryQuickLinksAsync(libraryName);
            
            string view = "~/Pages/Shared/Components/Mini/_miniQuickLinks.cshtml";

            return View(view, modelList);
        }

    }
}
