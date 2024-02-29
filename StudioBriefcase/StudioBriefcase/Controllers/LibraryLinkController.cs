using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace StudioBriefcase.Controllers
{
    //[ApiController]
    [Route("api/librarylink")]
    public class LibraryLinkController : Controller
    {
        private readonly LibraryService _libraryService;
        private readonly IPostTypeService _postTypeService;


        public LibraryLinkController(LibraryService libraryService, IPostTypeService postTypeService)
        {

            _libraryService = libraryService;
            _postTypeService = postTypeService;

        }

        [HttpPost("InitializeMiniMapSelectors")]
        public async Task<IActionResult> InitializeMiniMapSelectors([FromBody] JavascriptMapIDsModel targetIDs)
        {
            
            //I want to cache the ID values on client side, pass them to server. If IDs are 0, Create new lists.
            LibraryMapModel map = new LibraryMapModel(targetIDs.map);
            if(targetIDs.map.CategoryID == 0)
            {
                map.lists.Categories = await _libraryService.GetCategoryListAsync(targetIDs.LanguageID);
                map.lists.Libraries = await _libraryService.GetLibraryListAsync(map.lists.Categories.list[0].id, targetIDs.LanguageID);
                map.lists.Subjects = await _libraryService.GetSubjectListAsync(map.lists.Libraries.list[0].id, targetIDs.LanguageID);
                map.lists.Topics = await _libraryService.GetTopicListAsync(map.lists.Subjects.list[0].id, targetIDs.LanguageID);
            }
            else
            {
                map.lists.Categories = await _libraryService.GetCategoryListAsync(targetIDs.LanguageID);
                map.lists.Libraries = await _libraryService.GetLibraryListAsync(targetIDs.map.CategoryID, targetIDs.LanguageID);
                map.lists.Subjects = await _libraryService.GetSubjectListAsync(targetIDs.map.LibraryID, targetIDs.LanguageID);
                map.lists.Topics = await _libraryService.GetTopicListAsync(targetIDs.map.SubjectID, targetIDs.LanguageID);
            }

            return PartialView("~/Pages/Shared/Components/Page/_Page_MiniSelects.cshtml", map);
        }

        [HttpPost("GetVideoPostList")]
        public async Task<IActionResult> GetVideoPostList([FromBody] NavigationMapModel map)
        {
            List<string> list = await _libraryService.GetPostLinksAsync(map);
            return Ok(list);
        }
        /// <summary>
        /// TODO: DUPLICATE FUNCTION - REMOVE ONCE I LEARN JAVASCRIPT PATTERNS
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("UpdateLibraryOptions")]
        public async Task<IActionResult> UpdateLibraryOptions([FromBody] JavascriptMapIDsModel data)
        {
            LibraryMapModel temp = new LibraryMapModel(data.map);
              
            temp.lists.Categories = await _libraryService.GetCategoryListAsync(data.LanguageID);
            temp.lists.Libraries = await _libraryService.GetLibraryListAsync(data.map.CategoryID, data.LanguageID);
            if (data.map.LibraryID == 0)
            {
                temp.ids.LibraryID = temp.lists.Libraries.list[0].id;
            }

            temp.lists.Subjects = await _libraryService.GetSubjectListAsync(temp.ids.LibraryID, data.LanguageID);
            if (data.map.SubjectID == 0)
            {
                temp.ids.SubjectID = temp.lists.Subjects.list[0].id;
            }

            temp.lists.Topics = await _libraryService.GetTopicListAsync(temp.ids.SubjectID, data.LanguageID);

            return PartialView("~/Pages/Shared/Components/Page/_Page_MiniSelects.cshtml", temp);
        }

        [HttpPost("GetLibraryOptions")]
        public async Task<IActionResult> GetLibraryOptions([FromBody] JavascriptMapIDsModel data)
        {
            LibraryMapModel temp = new LibraryMapModel(data.map);

            Console.WriteLine($"CategoryID: {data.map.CategoryID}, LibraryID: {data.map.LibraryID}, SubjectID: {data.map.SubjectID}, TopicID: {data.map.TopicID}");

            temp.lists.Categories = await _libraryService.GetCategoryListAsync(data.LanguageID);
            temp.lists.Libraries = await _libraryService.GetLibraryListAsync(data.map.CategoryID, data.LanguageID);
            if (data.map.LibraryID == 0)
            {
                temp.ids.LibraryID = temp.lists.Libraries.list[0].id;
            }

            temp.lists.Subjects = await _libraryService.GetSubjectListAsync(temp.ids.LibraryID, data.LanguageID);
            if (data.map.SubjectID == 0)
            {
                temp.ids.SubjectID = temp.lists.Subjects.list[0].id;
            }

            temp.lists.Topics = await _libraryService.GetTopicListAsync(temp.ids.SubjectID, data.LanguageID);


            //return PartialView("~/Pages/Shared/Components/Library/_LibrarySelectorOptions", temp);
            return PartialView("~/Pages/Shared/Components/Library/_LibraryMapSelectors.cshtml", temp);
        }


        /// <summary> MOVE TO NEW SERVICE - Doesn't Connect to the database
        /// Build a Model with a data from the database and returns a Partial View with Model Data.
        /// </summary>
        [HttpPost("GetPreviewVideo")]
        public async Task<IActionResult> GetPreviewVideo([FromBody] ID_String_Pair_Model data)
        {

            var videomodel = await _postTypeService.GetYoutubePreview(data.text);
            if (videomodel == null)
            {

                return PartialView("~/Pages/Shared/Components/Posts/_previewDead.cshtml", data);
            }
            else
            {
                videomodel.sectionID = data.id;
            }

            return PartialView("~/Pages/Shared/Components/Posts/_previewVideo.cshtml", videomodel);
        }



        [HttpPost("GetPostDetailsForm")]
        public async Task<IActionResult> GetPostDetailsForm([FromBody] ClientSideSendingData clientData)
        {
            
            uint TopicID = clientData.topicID;
            string viewstring = "~/Pages/Shared/Components/Posts/_PostDetailsForm.cshtml"; //"~/Pages/Shared/Components/Posts/_PostDetailsForm.cshtml and _PostCreationForm.cshtml"

            uint postID = await _libraryService.PostTypeExistsAsync(clientData.url, "video");

            PostInspectModel model = new PostInspectModel(clientData.url);

            model.Tags.TagLists = await _libraryService.GetLibraryTagsAsync();

            //If an ID is found, then we can retrieve data on the post.
            if (postID != 0)
            {
                model.exists = true;
                model.Post = await _libraryService.GetPostIDValues(postID);
                if (model.Post != null)
                {                  
                    model.Tags.ids = await _libraryService.GetPostTagsAsync(postID);


                }

                model.language = new LanguageModel(model.Post!.post_language_id);
                
            }
            else {
                model.Post.section = clientData.sectionID;
                model.Post.post_language_id = clientData.language; //Phase Out of Model
                model.language = new LanguageModel(clientData.language);
            }

            model.language.options = await _libraryService.GetLanguages();

            model.Map = new LibraryMapModel(await _libraryService.BuildMapFromTopicID(TopicID));
            model.Map.lists.Categories = await _libraryService.GetCategoryListAsync(model.language.selectedID);
            model.Map.lists.Libraries = await _libraryService.GetLibraryListAsync(model.Map.ids.CategoryID, model.language.selectedID);
            model.Map.lists.Subjects = await _libraryService.GetSubjectListAsync(model.Map.ids.LibraryID, model.language.selectedID);
            model.Map.lists.Topics = await _libraryService.GetTopicListAsync(model.Map.ids.SubjectID, model.language.selectedID);
            //viewstring = "~/Pages/Shared/Components/Posts/_PostCreationForm.cshtml";



            //return PartialView(viewstring, pair);
            return PartialView(viewstring, model);
        }



        [HttpPost("InsertLink")]
        public async Task<IActionResult> InsertLink([FromBody] ClientInsertionData postLocation)
        {

            string SuccessMessage = string.Empty;


            if (uint.TryParse(User.FindFirst("sub")?.Value, out uint id))
            {
                postLocation.gitID = 0;
            }

            if (User.Identity?.IsAuthenticated == true || true)
            {
                switch (postLocation.post_type)
                {
                    case 1:
                        SuccessMessage = "Question Not Implemented";
                        break;
                    case 2:
                        SuccessMessage = "Book Links Not Implemented";
                        break;
                    case 3:
                        SuccessMessage = "Online Course Type Not Implemented";
                        break;
                    case 4:
                        SuccessMessage = "Series Type Not Implemented";
                        break;
                    case 5:
                        if (postLocation.url.StartsWith("https://youtu.be/"))
                        {
                            SuccessMessage = await _libraryService.InsertYoutubeLinkAsync(postLocation);

                        }
                        else
                        {
                            SuccessMessage = "Invalid Video Link";

                        }


                        break;
                    case 6:
                        SuccessMessage = "Website Not Implemented.";
                        break;
                    case 7:
                        SuccessMessage = "Answere Not Implemented.";
                        break;
                    default:
                        SuccessMessage = "Failed to Identify Type";
                        break;
                }


            }

            return Ok(SuccessMessage);
        }

        [HttpPost("DeletePost")]
        public async Task<IActionResult> DeletePost([FromBody] UINTTypeModel postID)
        {
            string message = "Failed to Delete: Unable to Authenticate Signin";

            //2 Conditions to delete a post are that the post is public, git_id of 0, or that the user has higher privelege to override regular use commands.
            //If the user is an Admin or Moderator, set gitID to 0 to treat the ID value as public status.
            //if (User.Identity?.IsAuthenticated == true)
            //{
                //uint gitID;
                //if (String.Equals(User.FindFirst("privilege")?.Value, "Admin", StringComparison.OrdinalIgnoreCase) || String.Equals(User.FindFirst("privilege")?.Value, "Moderator", StringComparison.OrdinalIgnoreCase))
                //{
                //    //IF user is admin or moderator, we can treat the query as if the post is a public entry
                //    gitID = 0;
                //}
                //else
                //{
                //    uint.TryParse(User.FindFirst("sub")?.Value, out gitID);
                //}

                message = await _libraryService.DeletePost(postID.uinttype, 0);
            //}


            return Ok(message);
        }


        [HttpPost("RetrieveRazorPage")]
        public async Task<IActionResult> RetrieveRazorPage([FromBody] ID_String_Pair_Model data)
        {
            
            //Working Example "/Library/Systems_Programming/CPP/Getting_Started/Introduction?iframe=true"
            string path = $"/Library/{await _libraryService.GetDirectyPathMap(data.id)}?iframe=true";

            return Ok(path);
        }
    }
}
