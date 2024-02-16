using Microsoft.AspNetCore.Mvc.RazorPages;

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
            //Console.WriteLine(Request.Path.Value);
            var dbmap = Request.Path.Value?.Split('/').ToArray();

            ViewData["Category"] = dbmap?[2];
            ViewData["Library"] = dbmap?[3];
            ViewData["Subject"] = dbmap?[4];
            ViewData["Topic"] = dbmap?[5];
            ViewData["TopicID"] = _topicID;
            
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
}
