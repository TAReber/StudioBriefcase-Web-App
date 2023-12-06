using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Shared
{
    public class SavedUserSettingsModel : PageModel
    {
        UserService _userService;

        public SavedUserSettingsModel(UserService userService)
        {
            
            _userService = userService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostTestMethod()
        {
            TestMethod();
            return RedirectToPage("/Shared/SavedUserSettings");
        }

        public void TestMethod()
        {
            Console.WriteLine("Hello World");
        }
    }
}
