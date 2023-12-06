using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IUserService
    {
        Task<bool> UserExistsAsync(uint user_id);
        Task AddUserAsync(UserModel user);
        Task<string> GetUserClass(uint user_id);
        Task<string> GetUserPrivilege(uint user_id);
        Task SetUserClass(uint userID, string userclass);
    }
}
