using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class LibraryLinksViewComponent : ViewComponent
    {
        PageService _pageService;
        //private ILibraryService _libraryService;
        //Constructor
        public LibraryLinksViewComponent(PageService pageService)
        {
            //_libraryService = libraryService;
            _pageService = pageService;
        }

        //TODO, I'll have to pass in a 
        public async Task<IViewComponentResult> InvokeAsync(string libraryName, int switchvalue = 0)
        {
            //Console.WriteLine("Called LibraryLink ViewComponent");
            var modelList = await _pageService.GetLibraryQuickLinksAsync(libraryName);

            string view = string.Empty;

            switch (switchvalue)
            {
                case 0:
                    view = "~/Pages/Shared/Components/Library/_libraryLinks.cshtml";
                    break;
                case 1:
                    view = "~/Pages/Shared/Components/Library/_libraryLinksEdit.cshtml";
                    break;
            }

            // View Takes in a view file and the list of models and returns a newly assembled block of html.
            return View(view, modelList);
        }
    }
}
