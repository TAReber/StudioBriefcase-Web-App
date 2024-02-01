using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;

namespace StudioBriefcase.ViewComponents
{
    public class LibraryTagsViewComponent : ViewComponent
    {
        ILibraryService _libraryService;

        public LibraryTagsViewComponent(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [ResponseCache(Duration =3600)] //Not Caching Response
        public async Task<IViewComponentResult> InvokeAsync(int FormID = 0)
        {

            LibraryTagsListModel tags = await _libraryService.GetLibraryTagsAsync();
            tags.pageid = FormID;
            return View("~/Pages/Shared/Components/Library/_LibraryPrebuiltTags.cshtml", tags);
        }
    }
}
