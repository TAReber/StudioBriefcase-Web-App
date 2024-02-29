﻿using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Models;

namespace StudioBriefcase.ViewComponents
{
    public class PageSection_Post : ViewComponent
    {
        public IViewComponentResult Invoke(int section)
        {

            //return View("~/Pages/Shared/Components/Page/_PageSection_Post.cshtml", section);
            //return Task.FromResult<IViewComponentResult>(View("~/Pages/Shared/Components/Page/_PageSection_Post.cshtml", section));
            return View("~/Pages/Shared/Components/Page/_PageSection_Post.cshtml", section);
        }
    }
}
