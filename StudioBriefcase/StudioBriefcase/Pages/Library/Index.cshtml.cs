using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library
{
    public class IndexModel : PageModel
    {

        public List<CategoryModel> CategoryList{ get; set; } = new List<CategoryModel>
        {
            new CategoryModel { Name = "Internet_Technology", Description = "WebSite Development & Internet"},
            new CategoryModel { Name = "Computer_Graphics", Description = "2D & 3D Graphics Libraries"},
            new CategoryModel { Name = "Systems_Programming", Description = "Low Level Languages"}
        };

        public void OnGet()
        {
        }
    }
}

