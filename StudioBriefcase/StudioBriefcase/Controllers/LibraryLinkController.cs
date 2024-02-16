using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;


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

        [HttpPost("GetVideoPostList")]
        public async Task<IActionResult> GetVideoPostList([FromBody] NavigationMapModel map)
        {
            List<string> list = await _libraryService.GetPostLinksAsync(map);
            return Ok(list);
        }

        [HttpPost("GetLibraryOptions")]
        public async Task<IActionResult> GetLibraryOptions([FromBody] LibraryMapIDsModel data)
        {
            LibraryMapListModel temp = new LibraryMapListModel(data);

            //Console.WriteLine($"CategoryID: {data.CategoryID}, LibraryID: {data.LibraryID}, SubjectID: {data.SubjectID}, TopicID: {data.TopicID}");

            temp.Categories = await _libraryService.GetCategoryListAsync();
            temp.Libraries = await _libraryService.GetLibraryListAsync(data.CategoryID);
            if (data.LibraryID == 0)
            {
                temp.LibraryID = temp.Libraries.list[0].id;
            }

            temp.Subjects = await _libraryService.GetSubjectListAsync(temp.LibraryID);
            if (data.SubjectID == 0)
            {
                temp.SubjectID = temp.Subjects.list[0].id;
            }

            temp.Topics = await _libraryService.GetTopicListAsync(temp.SubjectID);


            //return PartialView("~/Pages/Shared/Components/Library/_LibrarySelectorOptions", temp);
            return PartialView("~/Pages/Shared/Components/Library/_LibraryMapSelectors.cshtml", temp);
        }


        /// <summary> MOVE TO NEW SERVICE - Doesn't Connect to the database
        /// Build a Model with a data from the database and returns a Partial View with Model Data.
        /// </summary>
        [HttpPost("GetPreviewVideo")]
        public async Task<IActionResult> GetPreviewVideo([FromBody] ID_String_Pair_Model data)
        {
            Console.WriteLine(data.text);
            Console.WriteLine(data.id);
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

            }
            else {
                model.Post.section = clientData.sectionID;
                model.Post.post_language_id = clientData.language;
            }

            model.Map = new LibraryMapListModel(await _libraryService.BuildMapFromTopicID(TopicID));
            model.Map.Categories = await _libraryService.GetCategoryListAsync();
            model.Map.Libraries = await _libraryService.GetLibraryListAsync(model.Map.CategoryID);
            model.Map.Subjects = await _libraryService.GetSubjectListAsync(model.Map.LibraryID);
            model.Map.Topics = await _libraryService.GetTopicListAsync(model.Map.SubjectID);
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
    }
}
