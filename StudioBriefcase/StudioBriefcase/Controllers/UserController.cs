using Microsoft.AspNetCore.Mvc;
using StudioBriefcase.Services;
using StudioBriefcase.Models;
using System.Security.Claims;

namespace StudioBriefcase.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        //TODO:: Learn how to use ILogger and come up with some standardized system to log errors
        [HttpPost("UpdateUserClass")]
        public async Task<IActionResult> UpdateUserClass([FromBody] UserRoleUpdateModel model)
        {

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if(model.Id == User.FindFirst("sub")?.Value)
                {
                    if (uint.TryParse(model.Id, out uint id))
                    {
                        //Console.WriteLine(id);
                        await _userService.SetUserClass(id, model.userclass);
                        return Ok(new { Message = "Successfully Updated role to ", model.userclass });
                    }
                    else
                    {
                        return Ok(new { Message = "Failed to Change Role" });
                    }
                }
                else
                {
                    return Ok(new { Message = "Wrong User ID Submitted, Is this a Hack Attempt?" });
                }
            }
            return Ok(new { Message = "User Identity was Null, UserController.UpdateUserClass Method" });
        }

    }
}
