using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.Computer_Graphics.Vulkan.Vulkan_Overview
{
    public class Threading_BehaviorModel : BasePageModel
    {
        private const uint topicID = 7;

        public Threading_BehaviorModel(PageService pageService) : base(pageService, topicID)
        {
        }

        public async override Task<IActionResult?> OnGet(string language)
        {
            await base.OnGet(language);

            return Page();
        }
    }
}
