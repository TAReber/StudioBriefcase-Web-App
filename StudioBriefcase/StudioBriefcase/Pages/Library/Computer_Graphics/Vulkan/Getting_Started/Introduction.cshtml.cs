using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.ComputerGraphics.Vulkan.Overview
{
    public class IntroductionModel : BasePageModel
    {
        private const uint topicID = 1;

        public IntroductionModel(PageService pageService) : base(pageService, topicID)
        {
        }

        public async override Task<IActionResult?> OnGet(string language)
        {
            await base.OnGet(language);

            return Page();
        }
    }
}
