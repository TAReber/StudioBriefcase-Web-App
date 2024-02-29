using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library
{


    public class IndexModel : PageModel
    {

        PageService _pageService;
        public uint LanguageID { get; set; }
        public List<Map_Alias_Pair_Model> CategoryList { get; set; } = new List<Map_Alias_Pair_Model>();

        public IndexModel(PageService pageService)
        {
            
            _pageService = pageService;
        }

        public async Task<IActionResult> OnGet(string language)
        {
            
            uint langID = 0;


            if (language != null)
            {
                langID = await _pageService.LanguagesIDexists(language);

                // If the language is not found, default to Master Language
                if(langID == 0)
                {
                    string path = Request.Path.Value!;
                    string defaultPath = path.Replace($"/{language}", "");
                    return Redirect(defaultPath);
                }
            }


            if (langID == 0)
            {
                CategoryList = GetMasterCategories();
            }
            else
            {
                CategoryList = await _pageService.GetCategoryAliasIntros(langID);
                
            }
            LanguageID = langID;

            return Page();
        }

        private List<Map_Alias_Pair_Model> GetMasterCategories()
        {
            List<Map_Alias_Pair_Model> intros = new List<Map_Alias_Pair_Model>
            {
                new Map_Alias_Pair_Model(1, "Computer_Graphics", "Computer_Graphics", "2D & 3D Graphics Libraries"),
                new Map_Alias_Pair_Model(2, "Internet_Technology", "Internet_Technology", "WebSite Development & Internet"),
                new Map_Alias_Pair_Model(3, "Systems_Programming", "Systems_Programming", "Low Level Languages")
            };
            return intros;
        }

        public async Task<IActionResult> OnPostUpdateAlias(string aliasName, string aliasDescription, uint mapid, uint langid)
        {
            Console.WriteLine("Alias: " + aliasName + "desc: " + aliasDescription + " MapID: " + mapid + " LangID: " + langid);

            await _pageService.UpdateMapAliasAsync("categories", mapid, langid, aliasName, aliasDescription);

            LanguageID = langid;
            CategoryList = await _pageService.GetCategoryAliasIntros(langid);

            return Page();
        }


    }
}

