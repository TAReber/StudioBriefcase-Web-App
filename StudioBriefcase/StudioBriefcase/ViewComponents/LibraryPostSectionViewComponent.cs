using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;


namespace StudioBriefcase.ViewComponents
{
    public class LibraryPostSectionViewComponent : ViewComponent
    {
        LibraryService _libraryService;

        public LibraryPostSectionViewComponent(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public IViewComponentResult Invoke(int section)
        {
            LibrarySectionModel sectionData = new LibrarySectionModel(section);
            return View("~/Pages/Shared/Components/Library/_LibraryPostSection.cshtml", sectionData);
        }


    }
}
