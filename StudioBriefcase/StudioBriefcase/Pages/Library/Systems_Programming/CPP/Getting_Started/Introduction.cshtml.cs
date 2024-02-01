using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Services;
using System.Runtime.CompilerServices;

namespace StudioBriefcase.Pages.Library.Systems_Programming.CPP.Getting_Started
{
    public class IntroductionModel : PageModel
    {
        private ILibraryService? _libraryService;




        public IntroductionModel(ILibraryService libraryService)
        {

            
        }

        public void OnGet()
        {
            //string path = System.IO.Path.GetFileNameWithoutExtension(Request.Path);
            //Console.WriteLine($"Path: {path}");
            //string lastPath = Path.GetDirectoryName(Request.Path);
            //Console.WriteLine($"Last Path: {lastPath}");
        }
    }
}
