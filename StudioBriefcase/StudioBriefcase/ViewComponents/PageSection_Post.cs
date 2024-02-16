using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;

namespace StudioBriefcase.ViewComponents
{
    public class PageSection_Post : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int section)
        {

            LibrarySectionModel sectionData = new LibrarySectionModel(section);
            return View("~/Pages/Shared/Components/Page/_PageSection_Post.cshtml", sectionData);
            //return Task.FromResult<IViewComponentResult>(View("~/Pages/Shared/Components/Page/_PageSection_Post.cshtml", sectionData));
        }
    }
}
