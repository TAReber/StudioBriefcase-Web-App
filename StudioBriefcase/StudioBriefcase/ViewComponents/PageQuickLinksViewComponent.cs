using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class PageQuickLinksViewComponent : ViewComponent
    {
        PageService _pageService;

        public PageQuickLinksViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string libraryName, int switchvalue = 0)
        {
            var modelList = await _pageService.GetLibraryQuickLinksAsync(libraryName);

            string view = string.Empty;

            switch (switchvalue)
            {
                case 0:
                    view = "~/Pages/Shared/Components/Page/_PageQuickLinks.cshtml";
                    break;
                case 1:
                    view = "~/Pages/Shared/Components/Custodian/_Custodian_PageQuickLinks.cshtml";
                    break;
            }

            return View(view, modelList);
        }

    }
}
