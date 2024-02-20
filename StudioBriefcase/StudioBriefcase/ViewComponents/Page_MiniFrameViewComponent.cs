using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents
{
    public class Page_MiniFrameViewComponent : ViewComponent
    {
        PageService _pageService;

        public Page_MiniFrameViewComponent(PageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            LibraryMapIDsModel targetID = new LibraryMapIDsModel();
            LibraryMapModel map = new LibraryMapModel(targetID);
            map.lists.Categories = await _pageService.GetCategoryListAsync();
            map.lists.Libraries = await _pageService.GetLibraryListAsync(map.lists.Categories.list[0].id);
            map.lists.Subjects = await _pageService.GetSubjectListAsync(map.lists.Libraries.list[0].id);
            map.lists.Topics = await _pageService.GetTopicListAsync(map.lists.Subjects.list[0].id);

            return View("~/Pages/Shared/Components/Library/_LibraryMapSelectors.cshtml");
        }

    }
}
