using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudioBriefcase.Services;
using System.Security.Principal;

namespace StudioBriefcase.Models
{
    /// <summary>
    /// BasePageModel is the base class for all Libraries in the StudioBriefcase application
    /// </summary>
    public class BasePageModel : PageModel
    {
        private PageService _pageService;
        private uint _topicID;
        public uint LanguageID { get; set; } = 0;


        public BasePageModel(PageService pageservice, uint topicID)
        {
            _pageService = pageservice;
            _topicID = topicID;           
        }


        public async virtual Task<IActionResult?> OnGet(string language)
        {
            
            //uint langID = 0;
            if (language != null)
            {
                LanguageID = await _pageService.LanguagesIDexists(language);

                // If the language is not found, default to Master Language
                if (LanguageID == 0)
                {
                    string path = Request.Path.Value!;
                    string defaultPath = path.Replace($"/{language}", "");
                    return Redirect(defaultPath);
                }
            }

            CreateSubLayoutViewData();
     
            return Page();
        }

        private void CreateSubLayoutViewData()
        {
            bool iframe = Request.Query.TryGetValue("iframe", out var test);

            if (iframe)
            {
                ViewData["Layout"] = "/Pages/Shared/_Layout_iFrame.cshtml";
            }
            else
            {
                ViewData["Layout"] = "/Pages/Shared/_subLayout_Library.cshtml";
            }
            // Set Layout = ViewData["Layout"]?.ToString();

            //ViewData["iframe"] = iframe;

            var dbmap = Request.Path.Value!.Split('/').ToArray();
            ViewData["Category"] = dbmap![2];
            ViewData["Library"] = dbmap![3];
            ViewData["Subject"] = dbmap![4];
            ViewData["Topic"] = dbmap![5];
            //ViewData["Language"] = dbmap[6] ?? "Master";
            ViewData["LanguageID"] = LanguageID;
            ViewData["TopicID"] = _topicID;
        }

        public async Task<IActionResult> OnPostUpdateSubjectAlias(string aliasName, uint mapid, uint langid)
        {
            LanguageID = langid;
            CreateSubLayoutViewData();

            await _pageService.UpdateMapAliasNameAsync("subjects", mapid, langid, aliasName);
            

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateTopicAlias(string aliasName, uint mapid, uint langid)
        {
            LanguageID = langid;
            CreateSubLayoutViewData();

            await _pageService.UpdateMapAliasNameAsync("topics", mapid, langid, aliasName);

            return Page();
        }
    }

    public class PageQuickLinksJSONModel
    {
        public string LibraryName { get; set; } = string.Empty;
        public string JsonString { get; set; } = string.Empty;
    }

    public class PageQuickLinksModel
    { 
        public string SiteUrl { get; set; } = string.Empty;
        public string ImgSource { get; set; } = string.Empty;
        public string ShorthandDesc { get; set; } = string.Empty;
    }

    public class UINTTypeModel
    {
        public uint uinttype { get; set; }
    }

    public class ID_String_Pair_Model
    {
        public uint id { get; set; }
        public string text { get; set; } = string.Empty;
    }

    public class ID_String_Alias_Pair_Model
    {
        public uint id { get; set; }
        public string text { get; set; } = string.Empty;
        public string alias { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model used in the Index.cs pages, To be phased out for datadriven approach
    /// </summary>
    public class Map_Alias_Pair_Model
    {
        public ID_String_Pair_Model map { get; set; } = new ID_String_Pair_Model();
        public AliasNamesModel alias { get; set; } = new AliasNamesModel();

        public Map_Alias_Pair_Model() { 
            map = new ID_String_Pair_Model();
            alias = new AliasNamesModel();
        }

        public Map_Alias_Pair_Model(uint mapID, string mapName, string alias_name, string alias_description)
        { 
            map = new ID_String_Pair_Model { id = mapID, text = mapName };
            alias = new AliasNamesModel { name = alias_name, description = alias_description };
        }

        public Map_Alias_Pair_Model(ID_String_Pair_Model _map, AliasNamesModel _alias) {
            map = _map;
            alias = _alias;       
        }

    }

    public class AliasNamesModel
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }

    public class SubjectTopicsAliasEditorModel
    {
        public uint mapID { get; set; }
        public uint languageID { get; set; }
        public string topicName { get; set; } = string.Empty;
        public string topicAlias { get; set; } = string.Empty;
        public string subjectName { get; set; } = string.Empty;
        public string subjectAlias { get; set; } = string.Empty;
    }

    //Phasing Out
    public class staticlibraryLinkModel
    {
        
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

}
