using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class LibraryPostDataViewComponent : ViewComponent
    {
        ILibraryService _libraryService;

        public LibraryPostDataViewComponent(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string weblink, int postType)
        {

            
            return View();
        }

    }
}
