using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;

namespace StudioBriefcase.Pages.Library.ComputerGraphics.Vulkan.Getting_Started
{
    public class SDK_InstallationModel : BasePageModel
    {
        private const uint topicID = 2;

        public SDK_InstallationModel() : base(topicID)
        {

        }


        public override void OnGet()
        {
            base.OnGet();
        }
    }
}
