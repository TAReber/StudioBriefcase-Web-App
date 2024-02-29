using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.Internet_Technology.Web_Development.Getting_Started
{
    public class IntroductionModel : BasePageModel
    {
        private const uint topicID = 4;

        public IntroductionModel(PageService pageService) : base(pageService, topicID)
        {
        }

        public async override Task<IActionResult?> OnGet(string language)
        {           
            var page = base.OnGet(language);

            return await page;
        }
    }
}
