using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Computer_Graphics
{
    public class IndexModel : PageModel
    {
        //Uses a predefined Folder Heiarchy to Search for images and other related files to display thumbnails.
        public List<staticlibraryLinkModel> LibraryList { get; set; } = new List<staticlibraryLinkModel>
        {
            new staticlibraryLinkModel { Name = "Vulkan", Description = "Graphics API"}
        };

        public void OnGet()
        {
        }
    }
}
