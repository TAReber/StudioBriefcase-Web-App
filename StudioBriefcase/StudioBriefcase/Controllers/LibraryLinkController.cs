using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
namespace StudioBriefcase.Controllers
{
    //[ApiController]
    [Route("api/librarylink")]
    public class LibraryLinkController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly IPostTypeService _postTypeService;


        public LibraryLinkController(ILibraryService libraryService, IPostTypeService postTypeService)
        {

            _libraryService = libraryService;
            _postTypeService = postTypeService;
        }

        [HttpPost("GetVideoPostList")]
        public async Task<IActionResult> GetVideoPostList([FromBody] NavigationMapModel map)
        {
            //Console.WriteLine(map.libraryName);
            List<string> list = await _libraryService.GetVideoListAsync(map);
            return Ok(list);
        }


        [HttpPost("GetPreviewVideo")]
        public async Task<IActionResult> GetPreviewVideo([FromBody] VideoPreviewFetchModel fetchdata)
        {
            
            var videomodel = await _postTypeService.GetYoutubePreview(fetchdata.videolink);
            if (videomodel == null) {
             
                return PartialView("~/Pages/Shared/Components/Posts/_previewDead.cshtml", fetchdata);
            }
            else
            {
                videomodel.sectionID = fetchdata.section;
            }
            
            return PartialView("~/Pages/Shared/Components/Posts/_previewVideo.cshtml", videomodel);
        }

        [HttpPost("GetPostDetailsForm")]
        public async Task<IActionResult> GetPostDetailsForm([FromBody] BaseMapModel postLocation)
        {

            string viewstring = string.Empty; //"~/Pages/Shared/Components/Posts/_PostDetailsForm.cshtml and _PostCreationForm.cshtml"
            try
            {
                BaseMapModel test = postLocation;

                bool exists = false;
                //exists = await _libraryService.VideoPostTypeExistsAsync(postLocation.weblink);
                exists = await _libraryService.PostTypeExistsAsync(postLocation.weblink, "video");
                if (exists)
                {
                    VideoDatabaseModel postModel = await _libraryService.GetVideoMapData(postLocation.weblink);
                    postModel.videoTags = await _libraryService.GetPostTagsAsync(postModel.postID);
                    viewstring = "~/Pages/Shared/Components/Posts/_PostDetailsForm.cshtml";
                    return PartialView(viewstring, postModel);
                }
                else
                {
                    viewstring = "~/Pages/Shared/Components/Posts/_PostCreationForm.cshtml";
                }

            }
            catch
            {
                //TODO viewstring = "~/Pages/Shared/Components/Posts/_RequestFailedForm.cshtml"
                Console.WriteLine("Failed to Retrieve Post Details Form");
            }
            

            return PartialView(viewstring, postLocation);
        }



        [HttpPost("InsertLink")]
        public async Task<IActionResult> InsertLink([FromBody] PostMappingDataModel postLocation)
        {

            string SuccessMessage = string.Empty;

            if (uint.TryParse(User.FindFirst("sub")?.Value, out uint id))
            {
                postLocation.GitID = id;
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                switch (postLocation.posttype)
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
                        if (postLocation.weblink.StartsWith("https://youtu.be/"))
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
            if (User.Identity?.IsAuthenticated == true)
            {
                uint gitID;
                if (String.Equals(User.FindFirst("privilege")?.Value, "Admin", StringComparison.OrdinalIgnoreCase) || String.Equals(User.FindFirst("privilege")?.Value, "Moderator", StringComparison.OrdinalIgnoreCase))
                {
                    //IF user is admin or moderator, we can treat the query as if the post is a public entry
                    gitID = 0;
                }
                else
                {
                    uint.TryParse(User.FindFirst("sub")?.Value, out gitID);
                }
                
                message = await _libraryService.DeletePost(postID.uinttype, gitID);
            }


            return Ok(message);
        }
    }
}
