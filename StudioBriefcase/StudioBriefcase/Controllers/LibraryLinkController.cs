using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;
namespace StudioBriefcase.Controllers
{
    [ApiController]
    [Route("api/librarylink")]
    public class LibraryLinkController : Controller
    {
        private readonly LibraryService _libraryService;

        public LibraryLinkController(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpPost("SetLibraryLinks")]
        public async Task<IActionResult> SetLibraryLinks([FromBody] LibraryLinksUpdateModel jsonObject)
        {
            //Console.WriteLine(jsonObject.LibraryName);
            //Console.WriteLine(jsonObject.JsonString);


            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.FindFirst("privilege")?.Value == "Moderator")
                {
                    try
                    {
                        await _libraryService.SetLibraryQuickLinksAsync(jsonObject.LibraryName, jsonObject.JsonString);
                        return Ok(new { Message = "Successfully Updated Library Links" });
                    }
                    catch (Exception e)
                    {
                        return Ok(new { Message = "Failed to Update Library Links" });
                    }
                }
                else
                {
                    return Ok(new { Message = "Incorrect privilege, Is this a Hack Attempt?" });
                }
            }
            return Ok(new { Message = "User Identity was Null, LibraryLinkController.SetLibraryLinks Method" });








        }

        
    }
}
