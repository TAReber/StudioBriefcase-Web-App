using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class LibraryNavigationViewComponent : ViewComponent
    {
        ILibraryService _libraryService;

        public LibraryNavigationViewComponent(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string path)
        {

            var listdata = await _libraryService.GetSubjectListAsync(path);
        
            return View("~/Pages/Shared/Components/Library/_LibraryNavigation.cshtml", listdata);
        }
    }
}
