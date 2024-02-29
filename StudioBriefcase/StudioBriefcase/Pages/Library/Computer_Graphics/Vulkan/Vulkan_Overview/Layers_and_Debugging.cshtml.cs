using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.Computer_Graphics.Vulkan.Vulkan_Overview
{
    public class Layers_and_DebuggingModel : BasePageModel
    {
        private const uint topicID = 6;

        public Layers_and_DebuggingModel(PageService pageService) : base(pageService, topicID)
        {
        }

        public async override Task<IActionResult?> OnGet(string language)
        {
            await base.OnGet(language);

            return Page();
        }
    }
}
