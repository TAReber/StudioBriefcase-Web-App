using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Internet_Technology
{
    public class IndexModel : PageModel
    {
        public List<staticlibraryLinkModel> LibraryList { get; set; } = new List<staticlibraryLinkModel>
        {
            new staticlibraryLinkModel { Name = "Web_Development", Description = "Todo:Description"}
        };

        public void OnGet()
        {
        }
    }
}
