using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Models;
using StudioBriefcase.Services;

namespace StudioBriefcase.Pages.Library.Systems_Programming
{
    public class IndexModel : PageModel
    {


            PageService _pageService;
            public List<Map_Alias_Pair_Model> LibraryList { get; set; } = new List<Map_Alias_Pair_Model>();
            private const uint _categoryID = 3;
            public uint LanguageID { get; set; }

            public IndexModel(PageService pageService)
            {
                _pageService = pageService;
            }

            //Uses a predefined Folder Heiarchy to Search for images and other related files to display thumbnails.


            public async Task<IActionResult> OnGet(string language)
            {
                uint langID = 0;

                if (language != null)
                {
                    langID = await _pageService.LanguagesIDexists(language);

                    // If the language is not found, default to Master Language
                    if (langID == 0)
                    {
                        string path = Request.Path.Value!;
                        string defaultPath = path.Replace($"/{language}", "");
                        return Redirect(defaultPath);
                    }
                }

                if (langID == 0)
                {
                    LibraryList = GetMasterLibraries();
                }
                else
                {
                    LibraryList = await _pageService.GetLibraryAliasIntros(_categoryID, langID);

                }
                LanguageID = langID;

                return Page();
            }

            private List<Map_Alias_Pair_Model> GetMasterLibraries()
            {
                List<Map_Alias_Pair_Model> intros = new List<Map_Alias_Pair_Model>
            {
                new Map_Alias_Pair_Model(_categoryID, "CPP", "CPP", "C++ Language"),

            };
                return intros;
            }

            public async Task<IActionResult> OnPostUpdateAlias(string aliasName, string aliasDescription, uint mapid, uint langid)
            {
                await _pageService.UpdateMapAliasAsync("libraries", mapid, langid, aliasName, aliasDescription);

                LanguageID = langid;
                LibraryList = await _pageService.GetLibraryAliasIntros(_categoryID, langid);

                return Page();
            }
        }
}
