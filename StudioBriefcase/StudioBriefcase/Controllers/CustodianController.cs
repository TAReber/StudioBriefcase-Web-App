using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;

namespace StudioBriefcase.Controllers
{
    [ApiController]
    [Route("api/custodianController")]
    public class CustodianController : Controller
    {
        ICustodianService _custodianService;

        public CustodianController(ICustodianService custodianService)
        {
            _custodianService = custodianService;
        }


        [HttpPost("SetLibraryQuickLinks")]
        public async Task<IActionResult> SetLibraryQuickLinks([FromBody] LibraryQuickLinksUpdaterModel jsonObject)
        {
            Console.WriteLine(jsonObject.LibraryName);
            Console.WriteLine(jsonObject.JsonString);

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.FindFirst("privilege")?.Value == "Moderator")
                {
                    try
                    {
                        await _custodianService.SetLibraryQuickLinksAsync(jsonObject.LibraryName, jsonObject.JsonString);
                        return Ok(new { Message = "Successfully Updated Library Links" });
                    }
                    catch (Exception e)
                    {
                        return Ok(new { Message = $"Failed to Update Library Links{e}" });
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
