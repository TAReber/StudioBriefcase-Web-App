using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Systems_Programming
{
    public class IndexModel : PageModel
    {

        public List<staticlibraryLinkModel> LibraryList { get; set; } = new List<staticlibraryLinkModel>
        {
            new staticlibraryLinkModel { Name = "CPP", Description = "C++ Tutorial"}
        };
        public void OnGet()
        {
        }
    }
}
