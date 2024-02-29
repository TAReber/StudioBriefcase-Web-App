using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;

namespace StudioBriefcase.ViewComponents
{
    public class Page_MenuViewComponent : ViewComponent
    {
        PageService _pageService;
        public Page_MenuViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            LanguageModel languages = new LanguageModel(0);
            languages.options = await _pageService.GetLanguages();
            
            return View("~/Pages/Shared/Components/Page/_Page_Menu.cshtml", languages);
        }

    }
}
