using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Computer_Graphics.Vulkan.Vulkan_Overview
{
    public class VK_ConventionsModel : BasePageModel
    {
        private const uint topicID = 5;

        public VK_ConventionsModel() : base(topicID)
        {
        }

        public override void OnGet()
        {
            base.OnGet();
        }
    }
}
