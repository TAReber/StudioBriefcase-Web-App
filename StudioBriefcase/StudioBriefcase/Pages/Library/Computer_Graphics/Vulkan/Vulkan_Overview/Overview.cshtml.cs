using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.ComputerGraphics.Vulkan.Vulkan_Overview
{
    public class OverviewModel : BasePageModel
    {
        private const uint topicID = 3;

        public OverviewModel() : base(topicID)
        {

        }

        public override void OnGet()
        {
            base.OnGet();
        }
    }
}
