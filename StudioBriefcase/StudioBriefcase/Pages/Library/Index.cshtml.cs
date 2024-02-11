using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library
{
    public class IndexModel : PageModel
    {

        public List<staticlibraryLinkModel> CategoryList{ get; set; } = new List<staticlibraryLinkModel>
        {
            new staticlibraryLinkModel { Name = "Internet_Technology", Description = "WebSite Development & Internet"},
            new staticlibraryLinkModel { Name = "Computer_Graphics", Description = "2D & 3D Graphics Libraries"},
            new staticlibraryLinkModel { Name = "Systems_Programming", Description = "Low Level Languages"}
        };

        public void OnGet()
        {
        }
    }
}

