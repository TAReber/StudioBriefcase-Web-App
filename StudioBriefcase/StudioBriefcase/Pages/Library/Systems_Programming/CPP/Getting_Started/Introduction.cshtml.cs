using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;
using System.Runtime.CompilerServices;

namespace StudioBriefcase.Pages.Library.Systems_Programming.CPP.Getting_Started
{
    public class IntroductionModel : BasePageModel
    {
        
        private readonly PageService _pageService;
        public IntroductionModel(PageService pageService) : base(8)
        {
            _pageService = pageService;
        }      

        public override void OnGet()
        {
            base.OnGet();
            

        }
    }
}
