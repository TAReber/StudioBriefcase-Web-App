using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IPageService
    {
        Task<List<LibraryQuickLinksModel>> GetLibraryQuickLinksAsync(string libraryName);
        Task<List<SubjectModel>> GetSubjectListAsync(string path);
        Task<LibraryTagsListModel> GetLibraryTagsAsync();
    }
}
