using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Services;
using System.Runtime.CompilerServices;

namespace StudioBriefcase.Pages.Library.Systems_Programming.CPP.Getting_Started
{
    public class IntroductionModel : PageModel
    {
        private readonly PageService _pageService;
        public IntroductionModel(PageService pageService)
        {
            _pageService = pageService;
        }
       
        



        //public IntroductionModel(ILibraryService libraryService)
        //{
        //    _libraryService = libraryService;
            
        //}

        public void OnGet()
        {
            Console.WriteLine(Request.Path.Value);

            //Console.WriteLine($"Language:{Language}");
            //string path = System.IO.Path.GetFileNameWithoutExtension(Request.Path);
            //Console.WriteLine($"Path: {path}");
            //string lastPath = Path.GetDirectoryName(Request.Path);
            //Console.WriteLine($"Last Path: {lastPath}");

        }
    }
}
