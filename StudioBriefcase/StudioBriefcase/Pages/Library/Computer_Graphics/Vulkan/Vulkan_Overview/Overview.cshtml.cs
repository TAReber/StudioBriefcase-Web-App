using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.ComputerGraphics.Vulkan.Vulkan_Overview
{
    public class OverviewModel : BasePageModel
    {
        private const uint topicID = 3;

        public OverviewModel(PageService pageService) : base(pageService, topicID)
        {

        }

        public async override Task<IActionResult?> OnGet(string language)
        {
            await base.OnGet(language);

            return Page();
        }
    }
}
