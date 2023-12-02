using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class LibraryLinksViewComponent : ViewComponent
    {

        private ILibraryService _libraryService;
        //Constructor
        public LibraryLinksViewComponent(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        //TODO, I'll have to pass in a 
        public async Task<IViewComponentResult> InvokeAsync(string libraryName)
        {
            //Console.WriteLine("Called LibraryLink ViewComponent");
            var modelList = await _libraryService.GetLibraryLinksAsync(libraryName);
            // View Takes in a view file and the list of models and returns a newly assembled block of html.
            return View("~/Pages/Shared/Components/Library/_libraryLinks.cshtml", modelList);
        }
    }
}
