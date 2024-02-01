using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;

namespace StudioBriefcase.ViewComponents.PostTypes
{
    public class PostMappingViewComponent : ViewComponent
    {
        PostTypeService _postTypeService;
        public PostMappingViewComponent(PostTypeService posttypeservice)
        {
            _postTypeService = posttypeservice;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
