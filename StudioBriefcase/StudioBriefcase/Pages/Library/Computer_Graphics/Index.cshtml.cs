using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Computer_Graphics
{
    public class IndexModel : PageModel
    {
        public List<LibraryModel> LibraryList { get; set; } = new List<LibraryModel>
        {
            new LibraryModel { Name = "Vulkan", Description = "Graphics API"}
        };

        public void OnGet()
        {
        }
    }
}
