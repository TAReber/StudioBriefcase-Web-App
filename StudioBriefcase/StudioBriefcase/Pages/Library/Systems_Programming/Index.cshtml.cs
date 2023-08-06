using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Systems_Programming
{
    public class IndexModel : PageModel
    {

        public List<LibraryModel> LibraryList { get; set; } = new List<LibraryModel>
        {
            new LibraryModel { Name = "CPP", Description = "C++ Tutorial"}
        };
        public void OnGet()
        {
        }
    }
}
