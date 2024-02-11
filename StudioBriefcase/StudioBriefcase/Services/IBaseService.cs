using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IBaseService
    {
        Task<SelectorListModel> GetCategoryListAsync();
        

    }
}
