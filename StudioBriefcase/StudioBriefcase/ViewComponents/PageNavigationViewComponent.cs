using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class PageNavigationViewComponent : ViewComponent
    {
        PageService _pageService;

        public PageNavigationViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        /// <summary>
        /// Queries the Database for the list of subjects and topics to populate the library navigation panel
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(uint topicID, uint languageID)
        {

            SubLayoutNavigationModel listData = new SubLayoutNavigationModel(languageID);
            if(languageID == 0)
            {
                listData.Links = await _pageService.MakeSubLayoutNavigationLinksAsync(topicID);
            }
            else
            {
                listData = await _pageService.MakeSubLayoutNavigationLinksAsync(topicID, languageID);
            }

            

            return View("~/Pages/Shared/Components/Page/_Page_NavigationLinks.cshtml", listData);
        }
    }
}
