using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Shared.Forms
{
    public class SettingsPageModel : PageModel
    {
        UserService _userService;
        [BindProperty]
        public string _gitUserID { get; set; }
        [BindProperty]
        public string _userClass { get; set; }


        public SettingsPageModel(UserService userService)
        {
            _userClass = "Private";
            _gitUserID = "0";
            _userService = userService;
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPost()
        {
            //Console.WriteLine($"On Post Called {_gitUserID}. ");
            await UpdateDatabase();    
            return Page();
        }

        private async Task UpdateDatabase()
        {
            if (uint.TryParse(_gitUserID, out uint id))
            {
                //Console.WriteLine(id);
                await _userService.SetUserClass(id, _userClass);
            }
            else
            {
                //Console.WriteLine($"Invalid ID, {id}");
            }
        }


        public void OnPostTestMethod()
        {
            //Console.WriteLine("On Post Test Method Called");
            TestMethod();
        }

        public void TestMethod()
        {
            //Console.WriteLine("Hello World");
        }
    }
}
