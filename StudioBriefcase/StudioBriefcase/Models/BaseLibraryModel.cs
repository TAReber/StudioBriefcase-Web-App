using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Services;

namespace StudioBriefcase.Models
{
    public class BaseLibraryModel : PageModel
    {
        PageService _pageService;

        public BaseLibraryModel(PageService pageService, string Language)
        {
            _pageService = pageService;
        }

    }
}
