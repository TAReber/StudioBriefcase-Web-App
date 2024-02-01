using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Services;
using System.Net.Http;

namespace StudioBriefcase.Pages.Connect
{
    public class IndexModel : PageModel
    {
        //ILibraryService _libraryService;
        //IPostTypeService _postTypeService;
        //string library = string.Empty;
        //string subject = string.Empty;
        //string topic = string.Empty;
        //string sectiontest = string.Empty;

        //public IndexModel(ILibraryService libraryService, IPostTypeService postTypeService)
        //{
        //    _libraryService = libraryService;
        //    _postTypeService = postTypeService;
        //    //TODO:: Store File Path in variable so I can parse it later
        //    library = "CPP";
        //    subject = "Getting_Started";
        //    topic = "Introduction";

          
        //}

        public void OnGetAsync()
        {
            //var listdata = _postTypeService.GetVideoPreviewTestAsync();
            //foreach (var item in listdata.Result)
            //{
            //    Console.WriteLine(item.title);
            //}
        }



    }
}
