using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Internet_Technology
{
    public class IndexModel : PageModel
    {
        public List<LibraryModel> LibraryList { get; set; } = new List<LibraryModel>
        {
            new LibraryModel { Name = "Web_Development", Description = "Todo:Description"}
        };

        public void OnGet()
        {
        }
    }
}
