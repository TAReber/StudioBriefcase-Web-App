using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Internet_Technology.Web_Development.Getting_Started
{
    public class IntroductionModel : BasePageModel
    {
        private const uint topicID = 4;

        public IntroductionModel() : base(topicID)
        {
        }

        public override void OnGet()
        {
            base.OnGet();
        }
    }
}
