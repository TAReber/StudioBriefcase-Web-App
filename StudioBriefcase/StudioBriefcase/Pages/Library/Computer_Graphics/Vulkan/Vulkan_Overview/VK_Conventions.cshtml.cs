using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.Computer_Graphics.Vulkan.Vulkan_Overview
{
    public class VK_ConventionsModel : BasePageModel
    {
        private const uint topicID = 5;

        public VK_ConventionsModel(PageService pageService) : base(pageService, topicID)
        {
        }

        public async override Task<IActionResult?> OnGet(string language)
        {
            await base.OnGet(language);

            return Page();
        }
    }
}
