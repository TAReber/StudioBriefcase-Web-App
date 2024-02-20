using StudioBriefcase.Models;

namespace StudioBriefcase.Services
{
    public interface IBaseService
    {

        Task<LibraryMapIDsModel> BuildMapFromTopicID(uint topicID);
        Task<SelectorListModel> GetCategoryListAsync();
        Task<SelectorListModel> GetLibraryListAsync(uint categoryID);
        Task<SelectorListModel> GetSubjectListAsync(uint libraryID);
        Task<SelectorListModel> GetTopicListAsync(uint subjectID);

        Task<string> GetDirectyPathMap(uint TopicID);
        

        Task<LibraryTagsListModel> GetLibraryTagsAsync();
    }
}
