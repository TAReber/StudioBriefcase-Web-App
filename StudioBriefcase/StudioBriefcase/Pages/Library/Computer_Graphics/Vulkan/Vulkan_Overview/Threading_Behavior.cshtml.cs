using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Computer_Graphics.Vulkan.Vulkan_Overview
{
    public class Threading_BehaviorModel : BasePageModel
    {
        private const uint topicID = 7;

        public Threading_BehaviorModel() : base(topicID)
        {
        }

        public override void OnGet()
        {
            base.OnGet();
        }
    }
}
