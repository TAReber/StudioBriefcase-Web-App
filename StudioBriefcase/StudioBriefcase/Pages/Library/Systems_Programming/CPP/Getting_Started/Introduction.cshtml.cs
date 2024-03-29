using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;
using System.Runtime.CompilerServices;

namespace StudioBriefcase.Pages.Library.Systems_Programming.CPP.Getting_Started
{
    public class IntroductionModel : BasePageModel
    {       
        private const uint topicID = 8;
        
        public IntroductionModel(PageService pageService) : base(pageService, topicID)
        {
            
        }      

        public async override Task<IActionResult?> OnGet(string language)
        {
            await base.OnGet(language);

            return Page();
        }
    }
}
