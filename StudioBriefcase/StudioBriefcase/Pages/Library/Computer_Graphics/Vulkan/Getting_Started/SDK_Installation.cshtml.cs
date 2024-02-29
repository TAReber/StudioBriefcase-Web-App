using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.ComputerGraphics.Vulkan.Getting_Started
{
    public class SDK_InstallationModel : BasePageModel
    {
        private const uint topicID = 2;

        public SDK_InstallationModel(PageService pageService) : base(pageService, topicID)
        {

        }


        public async override Task<IActionResult?> OnGet(string language)
        {
            await base.OnGet(language);

            return Page();
        }
    }
}
