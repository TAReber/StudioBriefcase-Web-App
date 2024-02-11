using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;


namespace StudioBriefcase.ViewComponents
{
    public class LibraryPostSectionViewComponent : ViewComponent
    {
        //PageService _pageService;
        //LibraryService _libraryService;

        //public LibraryPostSectionViewComponent(PageService pageService)
        //{
        //    _pageService = pageService;
        //}

        public async Task<IViewComponentResult> InvokeAsync(int section)
        {
            //Console.WriteLine($"Called LibraryPostSectionViewComponent {section}");
            //if (await _pageService.PostTypeExistsAsync("https://youtu.be/90MeC-PTj50", "video") == true) { 
            //    Console.WriteLine("Video Post Exists");
            //}

            LibrarySectionModel sectionData = new LibrarySectionModel(section);
            return View("~/Pages/Shared/Components/Library/_LibraryPostSection.cshtml", sectionData);
            //return Task.FromResult<IViewComponentResult>(View("~/Pages/Shared/Components/Library/_LibraryPostSection.cshtml", sectionData));
        }


    }
}
