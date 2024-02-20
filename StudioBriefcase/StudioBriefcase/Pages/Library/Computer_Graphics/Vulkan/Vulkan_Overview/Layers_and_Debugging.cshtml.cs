using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.Computer_Graphics.Vulkan.Vulkan_Overview
{
    public class Layers_and_DebuggingModel : BasePageModel
    {
        private const uint topicID = 6;

        public Layers_and_DebuggingModel() : base(topicID)
        {
        }

        public override void OnGet()
        {
            base.OnGet();
        }
    }
}
