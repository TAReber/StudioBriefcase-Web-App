using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.ComputerGraphics.Vulkan.Overview
{
    public class IntroductionModel : BasePageModel
    {
        private const uint topicID = 1;

        public IntroductionModel() : base(topicID)
        {
        }

        public override void OnGet()
        {
            base.OnGet();
        }
    }
}
