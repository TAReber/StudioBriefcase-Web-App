using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IUserService
    {
        Task<bool> UserExistsAsync(uint user_id);
        Task AddUserAsync(UserModel user);
        Task<string> GetUserRole(uint user_id);
        Task SetUserRole(string role);
    }
}
