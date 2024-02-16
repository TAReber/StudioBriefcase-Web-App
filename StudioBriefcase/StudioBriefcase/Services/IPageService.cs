using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IPageService
    {
        Task<List<PageQuickLinksModel>> GetLibraryQuickLinksAsync(string libraryName);
        Task<List<SubjectModel>> GetSubjectListAsync(string path);
        
        //Task<PageMapModel> GetPageMapID(string category, string library, string subject, string topic);
    }
}
