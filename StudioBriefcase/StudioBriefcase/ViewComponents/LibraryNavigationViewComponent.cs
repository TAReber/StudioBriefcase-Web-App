using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class LibraryNavigationViewComponent : ViewComponent
    {

        PageService _pageService;

        public LibraryNavigationViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        /// <summary>
        /// Queries the Database for the list of subjects and topics to populate the library navigation panel
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(string path)
        {

            var listdata = await _pageService.GetSubjectListAsync(path);
        
            return View("~/Pages/Shared/Components/Library/_LibraryNavigation.cshtml", listdata);
        }
    }
}
