using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;

namespace StudioBriefcase.Models
{
    /// <summary>
    /// BasePageModel is the base class for all Libraries in the StudioBriefcase application
    /// </summary>
    public class BasePageModel : PageModel
    {
        private uint _topicID;



        public BasePageModel(uint topicID)
        {
            _topicID = topicID;           
        }


        public virtual void OnGet()
        {
            
            bool iframe = Request.Query.TryGetValue("iframe", out var test);

            if (iframe)
            {
                ViewData["Layout"] = "/Pages/Shared/_Layout_iFrame.cshtml";
            }
            else
            {
                ViewData["Layout"] = "/Pages/Shared/_subLayout_Library.cshtml";
            }
            // Set Layout = ViewData["Layout"]?.ToString();

            ViewData["iframe"] = iframe;

            var dbmap = Request.Path.Value?.Split('/').ToArray();
            ViewData["Category"] = dbmap?[2];
            ViewData["Library"] = dbmap?[3];
            ViewData["Subject"] = dbmap?[4];
            ViewData["Topic"] = dbmap?[5];
            ViewData["TopicID"] = _topicID;

            //Console.WriteLine(dbmap?[2]);
            
        }
    }

    public class PageQuickLinksJSONModel
    {
        public string LibraryName { get; set; } = string.Empty;
        public string JsonString { get; set; } = string.Empty;
    }

    public class PageQuickLinksModel
    { 
        public string SiteUrl { get; set; } = string.Empty;
        public string ImgSource { get; set; } = string.Empty;
        public string ShorthandDesc { get; set; } = string.Empty;
    }

    public class UINTTypeModel
    {
        public uint uinttype { get; set; }
    }
}
