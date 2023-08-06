using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface ILibraryService
    {
        
        Task<List<LibraryLinksModel>> GetLibraryLinksAsync(string libraryName);
        Task<List<SubjectModel>> GetSubjectListAsync(string subjectName);
        
    }
}
